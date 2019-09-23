using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Queue;
using FiiiPay.Wallet.Business.Crypotcurrencies;
using log4net;
using Newtonsoft.Json;
using System;
using System.Transactions;
using TransactionStatus = FiiiPay.Entities.Enums.TransactionStatus;

namespace FiiiPay.Wallet.Business
{
    public class MiningComponent
    {
        private static int cryptoId = 0;
        private static int FirstLevelCount = 3;
        private static int SecondLevelCount = 10;
        private static int ThirdLevelCount = 30;
        private static decimal FirstLevel = 0.04m;
        private static decimal SecondLevel = 0.06m;
        private static decimal ThirdLevel = 0.08m;

        private readonly ILog _logger = LogManager.GetLogger("LogicError");
        private const int DbIndex = 0;
        private string listKey = "Mining:MiningConfirmed:List";
        private string miningReceivedKey = "Mining:MiningReceived:RequestId:";
        public void MiningReceived(long requestId, string transactionId, byte accountType, Guid accountId, decimal amount)
        {
            switch (accountType)
            {
                case (byte)AccountType.User:

                    var userAccount = new Data.UserAccountDAC().GetById(accountId);
                    if (userAccount == null)
                        throw new CommonException(20000, "Error: Invalid user account");

                    var _userWalletFactory = new UserWalletFactory();
                    var _userDepositFactory = new UserDepositFactory();
                    var _userWalletStatementFactory = new UserWalletStatementFactory();

                    MiningReceived(_userWalletFactory, _userDepositFactory, _userWalletStatementFactory, new UserTransactionFactory(), requestId, transactionId, accountId, amount);
                    break;

                case (byte)AccountType.Merchant:

                    var merchantAccount = new Data.MerchantAccountDAC().GetById(accountId);
                    if (merchantAccount == null)
                        throw new CommonException(20000, "Error: Invalid merchant account");

                    var _walletFactory = new MerchantWalletFactory();
                    var _depositFactory = new MerchantDepositFactory();
                    var _walletStatementFactory = new MerchantWalletStatementFactory();

                    MiningReceived(_walletFactory, _depositFactory, _walletStatementFactory, new MerchantTransactionFactory(), requestId, transactionId, accountId, amount);
                    break;
            }
        }

        private void MiningReceived(WalletFactory _walletFactory, DepositFactory _depositFactory, WalletStatementFactory _walletStatementFactory, TransactionFactory _transactionFactory,
            long requestId, string transactionId, Guid accountId, decimal amount)
        {
            if (RedisHelper.KeyExists(miningReceivedKey + requestId))//防止没写进数据库之前，相同的请求调用
            {
                throw new CommonException(20001, "Error: Repeated requests");
            }

            var excistDeposit = _depositFactory.GetByRequestId(accountId, requestId);
            if (excistDeposit != null)
                throw new CommonException(20001, "Error: Repeated requests");

            RedisHelper.StringSet(miningReceivedKey + requestId, "1", new TimeSpan(0, 0, 30));

            var crypto = new CryptocurrencyDAC().GetByCode("FIII");
            if (crypto == null)
                throw new CommonException(20000, "Error: Invalid cryptocurrency");

            var wallet = _walletFactory.GetByAccountId(accountId, crypto.Id);

            var timestamp = DateTime.UtcNow;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = GenerateWallet(_walletFactory, accountId, crypto.Id, crypto.Code);

                var deposit = new Deposit
                {
                    AccountId = accountId,
                    WalletId = wallet.Id,
                    FromType = DepositFromType.PosMining,
                    ToAddress = wallet.Address,
                    Amount = amount,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = timestamp,
                    RequestId = requestId,
                    OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                    TransactionId = transactionId,
                    CryptoCode = crypto.Code
                };
                _depositFactory.Insert(deposit);
                _transactionFactory.Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
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
                //_walletFactory.DecreaseFreeze(wallet.Id, -amount);
                _walletStatementFactory.Insert(new WalletStatement
                {
                    Action = Entities.UserWalletStatementAction.AwardDeposit,
                    Amount = amount,
                    Balance = wallet.Balance + amount,
                    FrozenAmount = 0,
                    FrozenBalance = wallet.FrozenBalance,
                    Remark = null,
                    Timestamp = timestamp,
                    WalletId = wallet.Id
                });
                scope.Complete();
            }
        }

        private WalletEntity GenerateWallet(WalletFactory _walletFactory, Guid accountId, int idValue, string cryptoCode)
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

        //public void MiningConfirmed(List<Tuple<byte, Guid, decimal>> accountList)
        //{
        //    if (accountList == null || accountList.Count <= 0)
        //        return;

        //    MiningInfo info;
        //    foreach (var item in accountList)
        //    {
        //        info = new MiningInfo { AccountType = item.Item1, AccountId = item.Item2, Amount = item.Item3 };
        //        RabbitMQSender.SendMessage("MiningConfirmed", JsonConvert.SerializeObject(info));
        //    }
        //}

        public void MiningConfirmed(byte accountType, Guid accountId, decimal amount)
        {
            var info = new MiningInfo { AccountType = accountType, AccountId = accountId, Amount = amount };
            RabbitMQSender.SendMessage("MiningConfirmed", JsonConvert.SerializeObject(info));
        }
    }
}
