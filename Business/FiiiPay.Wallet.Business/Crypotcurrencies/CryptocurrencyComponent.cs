using System;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Wallet.Business.Properties;
using log4net;
using TransactionStatus = FiiiPay.Entities.Enums.TransactionStatus;

// ReSharper disable once CheckNamespace
namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public class CryptocurrencyComponent
    {
        private readonly AccountFactory _accountFactory;
        private readonly DepositFactory _depositFactory;
        private readonly WithdrawFactory _withdrawFactory;
        private readonly WalletFactory _walletFactory;
        private readonly RedisMQFactory _redisMqFactory;
        private readonly CryptocurrencyDAC _cryptocurrencyDac;
        private readonly WalletStatementFactory _walletStatementFactory;
        private readonly TransactionFactory _transactionFactory;

        private readonly ILog _log = LogManager.GetLogger(typeof(CryptocurrencyComponent));

        public CryptocurrencyComponent(AccountType accountType)
        {
            _cryptocurrencyDac = new CryptocurrencyDAC();
            switch (accountType)
            {
                case AccountType.User:
                    _accountFactory = new UserAccountFactory();
                    _depositFactory = new UserDepositFactory();
                    _walletFactory = new UserWalletFactory();
                    _redisMqFactory = new UserRedisMQFactory();
                    _withdrawFactory = new UserWithdrawFactory();
                    _walletStatementFactory = new UserWalletStatementFactory();
                    _transactionFactory = new UserTransactionFactory();
                    break;
                case AccountType.Merchant:
                    _accountFactory = new MerchantAccountFactory();
                    _depositFactory = new MerchantDepositFactory();
                    _walletFactory = new MerchantWalletFactory();
                    _redisMqFactory = new MerchantRedisMQFactory();
                    _withdrawFactory = new MerchantWithdrawFactory();
                    _walletStatementFactory = new MerchantWalletStatementFactory();
                    _transactionFactory = new MerchantTransactionFactory();
                    break;
                default:
                    throw new CommonException(20000, "Error: Invalid Account Type");
            }
        }

        public void Deposit(Guid accountId, string transactionId, decimal amount, string cryptoName, DateTime timestamp, string address, string tag)
        {
            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");

            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            Deposit deposit;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = GenerateWallet(accountId, crypto.Id, crypto.Code);

                deposit = new Deposit
                {
                    AccountId = account.Id,
                    WalletId = wallet.Id,
                    FromAddress = address,
                    FromTag = tag,
                    ToAddress = wallet.Address,
                    ToTag = wallet.Tag,
                    Amount = amount,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = timestamp,
                    OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                    TransactionId = transactionId
                };
                deposit = _depositFactory.Insert(deposit);
                _walletFactory.Increase(wallet.Id, amount);
                _walletStatementFactory.Insert(new WalletStatement
                {
                    Action = MerchantWalletStatementAction.Deposit,
                    Amount = amount,
                    Balance = wallet.Balance + amount,
                    Remark = null,
                    Timestamp = DateTime.UtcNow,
                    WalletId = wallet.Id
                });
                scope.Complete();
            }

            try
            {
                _redisMqFactory.PubDeposit(deposit.Id);

                if (!string.IsNullOrEmpty(account.Email))
                {
                    string subject = string.Format(Resources.DepositTitle, cryptoName);
                    string content = string.Format(Resources.DepositEmailContent, amount, cryptoName);
                    new EmailAgent().Send(account.Email, subject, content, 5);
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void DepositPending(Guid accountId, long requestId, string transactionId, decimal amount, string cryptoName, string address, string tag)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new CommonException(20000, "Invalid TransactionId");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");

            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            //防止重复处理
            var deposit = _depositFactory.GetByRequestId(accountId, requestId);
            if (deposit != null)
                throw new CommonException(20000, "Error: Duplicate RequestId");

            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = GenerateWallet(accountId, crypto.Id, crypto.Code);

                var depositObj = _depositFactory.Insert(new Deposit
                {
                    AccountId = account.Id,
                    WalletId = wallet.Id,
                    FromAddress = address,
                    FromTag = tag,
                    ToAddress = wallet.Address,
                    ToTag = wallet.Tag,
                    Amount = amount,
                    Status = TransactionStatus.Pending,
                    Timestamp = DateTime.UtcNow,
                    OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                    TransactionId = transactionId,
                    RequestId = requestId,
                    CryptoCode = crypto.Code,
                    FromType = DepositFromType.Deposit
                });

                _transactionFactory.Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    CryptoId = crypto.Id,
                    CryptoCode = crypto.Code,
                    Type = UserTransactionType.Deposit,
                    DetailId = depositObj.Id.ToString(),
                    Status = (byte)depositObj.Status,
                    Timestamp = depositObj.Timestamp,
                    Amount = amount,
                    OrderNo = depositObj.OrderNo
                });

                scope.Complete();
            }
        }

        public void DepositCompleted(Guid accountId, long depositRequestId, string cryptoName, string transactionId, decimal amount, string address, string tag)
        {
            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");

            var deposit = _depositFactory.GetByRequestId(accountId, depositRequestId);
            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            //充值订单存在 完成订单
            if (deposit != null)
            {
                if (deposit.Status == TransactionStatus.Confirmed) return;

                if (deposit.Status != TransactionStatus.Pending)
                    throw new CommonException(20000, "Error: Invalid Deposit Status");

                using (var scope = new TransactionScope())
                {
                    _walletFactory.Increase(deposit.WalletId, deposit.Amount);
                    _depositFactory.CompletedByRequestId(accountId, wallet.Id, depositRequestId);
                    _transactionFactory.UpdateStatus(UserTransactionType.Deposit, deposit.Id.ToString(), accountId, (byte)TransactionStatus.Confirmed);
                    _walletStatementFactory.Insert(new WalletStatement
                    {
                        Action = MerchantWalletStatementAction.Deposit,
                        Amount = amount,
                        Balance = wallet.Balance + amount,
                        FrozenAmount = 0,
                        FrozenBalance = wallet.FrozenBalance,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = wallet.Id
                    });
                    scope.Complete();
                }
            }
            //充值订单不存在 创建一条已完成订单
            else
            {
                if (string.IsNullOrEmpty(transactionId))
                    throw new CommonException(20000, "Invalid TransactionId");

                using (var scope = new TransactionScope())
                {
                    if (wallet == null)
                        wallet = GenerateWallet(account.Id, crypto.Id, crypto.Code);

                    deposit = _depositFactory.Insert(new Deposit
                    {
                        AccountId = account.Id,
                        WalletId = wallet.Id,
                        FromAddress = address,
                        FromTag = tag,
                        ToAddress = wallet.Address,
                        ToTag = wallet.Tag,
                        Amount = amount,
                        Status = TransactionStatus.Confirmed,
                        Timestamp = DateTime.UtcNow,
                        OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                        TransactionId = transactionId,
                        RequestId = depositRequestId,
                        CryptoCode = crypto.Code,
                        FromType = DepositFromType.Deposit
                    });
                    _transactionFactory.Insert(new UserTransaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = account.Id,
                        CryptoId = crypto.Id,
                        CryptoCode = crypto.Code,
                        Type = UserTransactionType.Deposit,
                        DetailId = deposit.Id.ToString(),
                        Status = (byte)deposit.Status,
                        Timestamp = deposit.Timestamp,
                        Amount = amount,
                        OrderNo = deposit.OrderNo
                    });
                    _walletFactory.Increase(wallet.Id, amount);
                    _walletStatementFactory.Insert(new WalletStatement
                    {
                        Action = MerchantWalletStatementAction.Deposit,
                        Amount = amount,
                        Balance = wallet.Balance + amount,
                        FrozenAmount = 0,
                        FrozenBalance = wallet.FrozenBalance,
                        Remark = null,
                        Timestamp = DateTime.UtcNow,
                        WalletId = wallet.Id
                    });
                    scope.Complete();
                }
            }
            try
            {
                _redisMqFactory.PubDeposit(deposit.Id);

                //if (!string.IsNullOrEmpty(account.Email))
                //{
                //    string subject = string.Format(Resources.DepositTitle, cryptoName);
                //    string content = string.Format(Resources.DepositEmailContent, deposit.Amount.Value, cryptoName);
                //    new EmailAgent().Send(account.Email, subject, content, 5);
                //}

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

        }

        public void DepositCancel(Guid accountId, long depositRequestId, string cryptoName)
        {
            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");

            var deposit = _depositFactory.GetByRequestId(accountId, depositRequestId);
            if (deposit == null)
                throw new CommonException(20000, "Error: Invalid RequestId");
            if (deposit.Status != TransactionStatus.Pending)
                throw new CommonException(20000, "Error: Invalid Deposit Status");

            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            using (var scope = new TransactionScope())
            {
                _depositFactory.CancelByRequestId(accountId, wallet.Id, depositRequestId);
                _transactionFactory.UpdateStatus(UserTransactionType.Deposit, deposit.Id.ToString(), accountId, (byte)TransactionStatus.Cancelled);
                scope.Complete();
            }

            _redisMqFactory.PubDepositCancel(deposit.Id);
        }

        public void WithdrawCompleted(Guid accountId, long withdrawRequestId, string cryptoName, string transactionId)
        {
            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");

            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            var withdrawal = _withdrawFactory.GetByRequestId(accountId, wallet.Id, withdrawRequestId);
            if (withdrawal == null)
                throw new CommonException(20000, "Error: Invalid RequestID");

            if (withdrawal.Status == TransactionStatus.Confirmed) return;

            if (withdrawal.Status != TransactionStatus.Pending)
                throw new CommonException(20000, "Error: Invalid Withdrawal Status");

            using (var scope = new TransactionScope())
            {
                _walletFactory.DecreaseFreeze(wallet.Id, withdrawal.Amount);

                _withdrawFactory.CompletedByRequestId(accountId, wallet.Id, withdrawRequestId, transactionId);
                _walletStatementFactory.Insert(new WalletStatement
                {
                    Action = MerchantWalletStatementAction.Withdrawal,
                    Amount = 0,
                    Balance = wallet.Balance,
                    FrozenAmount = -withdrawal.Amount,
                    FrozenBalance = wallet.FrozenBalance - withdrawal.Amount,
                    Remark = null,
                    Timestamp = DateTime.UtcNow,
                    WalletId = wallet.Id
                });
                _transactionFactory.UpdateStatus(UserTransactionType.Withdrawal, withdrawal.Id.ToString(), accountId, (byte)TransactionStatus.Confirmed);
                scope.Complete();
            }

            try
            {
                _redisMqFactory.PubWithdrawCompleted(withdrawal.Id);
                if (!string.IsNullOrEmpty(account.Email))
                {
                    string subject = string.Format(Resources.WithdrawalEmailTitle, cryptoName);
                    string content = string.Format(Resources.WithdrawalEmailContent, withdrawal.Amount, cryptoName);
                    new EmailAgent().Send(account.Email, subject, content, 5);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void WithdrawReject(Guid accountId, long withdrawRequestId, string cryptoName, string reason)
        {
            if (withdrawRequestId == 1982L || withdrawRequestId == 1983L || withdrawRequestId == 1984L)
            {
                _log.Info("ignore withdraw request " + withdrawRequestId);
                return;
            }

            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");
            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            var withdrawal = _withdrawFactory.GetByRequestId(accountId, wallet.Id, withdrawRequestId);
            if (withdrawal == null)
                throw new CommonException(20000, "Error: Invalid RequestID");

            if (withdrawal.Status == TransactionStatus.Confirmed || withdrawal.Status == TransactionStatus.Cancelled) return;

            if (withdrawal.Status != TransactionStatus.Pending)
                throw new CommonException(20000, "Error: Invalid Withdrawal Status");

            using (var scope = new TransactionScope())
            {
                var unfreezeAmount = withdrawal.Amount;

                if (withdrawal.Amount > wallet.FrozenBalance)
                {
                    unfreezeAmount = wallet.FrozenBalance;
                }

                _withdrawFactory.RejectByRequestId(accountId, wallet.Id, withdrawRequestId, reason);
                _transactionFactory.UpdateStatus(UserTransactionType.Withdrawal, withdrawal.Id.ToString(), accountId, (byte)TransactionStatus.Cancelled);
                //退款
                _walletFactory.Unfreeze(wallet.Id, withdrawal.Amount, unfreezeAmount);
                scope.Complete();
            }

            try
            {
                _redisMqFactory.PubWithdrawReject(withdrawal.Id);
                if (!string.IsNullOrEmpty(account.Email))
                {
                    string subject = string.Format(Resources.WithdrawalRejectEmailTitle, cryptoName);
                    string content = string.Format(Resources.WithdrawalRejectEmailContent, withdrawal.Amount, cryptoName);
                    new EmailAgent().Send(account.Email, subject, content, 5);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void WithdrawApproved(Guid accountId, long withdrawRequestId, string cryptoName, string transactionId)
        {
            var account = _accountFactory.GetById(accountId);
            if (account == null)
                throw new CommonException(20000, "Error: Invalid Account ID");

            var crypto = _cryptocurrencyDac.GetByCode(cryptoName);
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid Cryptocurrency");
            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            var withdrawal = _withdrawFactory.GetByRequestId(accountId, wallet.Id, withdrawRequestId);
            if (withdrawal == null)
                throw new CommonException(20000, "Error: Invalid RequestID");
            if (withdrawal.Status != TransactionStatus.Pending)
                throw new CommonException(20000, "Error: Invalid Withdrawal Status");

            _withdrawFactory.InitTransactionId(accountId, wallet.Id, withdrawRequestId, transactionId);
        }

        private Crypotcurrencies.WalletEntity GenerateWallet(Guid accountId, int idValue, string cryptoCode)
        {
            var wallet = new WalletEntity
            {
                AccountId = accountId,
                CryptoId = idValue,
                Balance = 0M,
                FrozenBalance = 0M,
                Address = null,
                CryptoCode = cryptoCode
            };
            return _walletFactory.Insert(wallet);
        }
    }
}