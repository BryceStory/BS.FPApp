using System;
using FiiiPay.Data;
using FiiiPay.Entities;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class DepositFactory
    {
        public abstract Deposit GetByRequestId(Guid accountId, long requestId);
        public abstract Deposit Insert(Deposit deposit);

        public abstract void CompletedByRequestId(Guid accountId, long walletId, long depositRequestId);

        public abstract void CancelByRequestId(Guid accountId, long walletId, long depositRequestId);
    }

    public class MerchantDepositFactory : DepositFactory
    {
        public override Deposit GetByRequestId(Guid accountId, long requestId)
        {
            var baseDeposit = new MerchantDepositDAC().GetByRequestId(accountId, requestId);
            if (baseDeposit == null)
                return null;
            return new Deposit
            {
                Id = baseDeposit.Id,
                AccountId = baseDeposit.MerchantAccountId,
                WalletId = baseDeposit.MerchantWalletId,
                FromAddress = baseDeposit.FromAddress,
                ToAddress = baseDeposit.ToAddress,
                Amount = baseDeposit.Amount,
                Status = baseDeposit.Status,
                Timestamp = baseDeposit.Timestamp,
                OrderNo = baseDeposit.OrderNo,
                TransactionId = baseDeposit.TransactionId,
                RequestId = baseDeposit.RequestId
            };
        }

        public override Deposit Insert(Deposit deposit)
        {
            var userDeposit = new MerchantDeposit
            {
                MerchantAccountId = deposit.AccountId,
                MerchantWalletId = deposit.WalletId,
                FromType = deposit.FromType,
                FromAddress = deposit.FromAddress,
                ToAddress = deposit.ToAddress,
                Amount = deposit.Amount,
                Status = deposit.Status,
                Timestamp = deposit.Timestamp,
                OrderNo = deposit.OrderNo,
                TransactionId = deposit.TransactionId,
                RequestId = deposit.RequestId,
                SelfPlatform = false,
                Remark = null,
                CryptoCode = deposit.CryptoCode
            };
            userDeposit = new MerchantDepositDAC().Insert(userDeposit);
            deposit.Id = userDeposit.Id;
            return deposit;
        }

        public override void CompletedByRequestId(Guid accountId, long walletId, long depositRequestId)
        {
            new MerchantDepositDAC().CompletedByRequestId(accountId, walletId, depositRequestId);
        }

        public override void CancelByRequestId(Guid accountId, long walletId, long depositRequestId)
        {
            new MerchantDepositDAC().CancelByRequestId(accountId, walletId, depositRequestId);
        }
    }

    public class UserDepositFactory : DepositFactory
    {
        public override Deposit GetByRequestId(Guid accountId, long requestId)
        {
            var baseDeposit = new UserDepositDAC().GetByRequestId(accountId, requestId);
            if (baseDeposit == null)
                return null;
            return new Deposit
            {
                Id = baseDeposit.Id,
                AccountId = baseDeposit.UserAccountId,
                WalletId = baseDeposit.UserWalletId,
                FromAddress = baseDeposit.FromAddress,
                ToAddress = baseDeposit.ToAddress,
                Amount = baseDeposit.Amount,
                Status = baseDeposit.Status,
                Timestamp = baseDeposit.Timestamp,
                OrderNo = baseDeposit.OrderNo,
                TransactionId = baseDeposit.TransactionId,
                RequestId = baseDeposit.RequestId
            };
        }

        public override Deposit Insert(Deposit deposit)
        {
            var userDeposit = new UserDeposit
            {
                UserAccountId = deposit.AccountId,
                UserWalletId = deposit.WalletId,
                FromType = deposit.FromType,
                FromAddress = deposit.FromAddress,
                FromTag = deposit.FromTag,
                ToAddress = deposit.ToAddress,
                ToTag = deposit.ToTag,
                Amount = deposit.Amount,
                Status = deposit.Status,
                Timestamp = deposit.Timestamp,
                OrderNo = deposit.OrderNo,
                TransactionId = deposit.TransactionId,
                RequestId = deposit.RequestId,
                SelfPlatform = false,
                Remark = null,
                CryptoCode = deposit.CryptoCode
            };
            userDeposit = new UserDepositDAC().Insert(userDeposit);
            deposit.Id = userDeposit.Id;
            return deposit;
        }

        public override void CompletedByRequestId(Guid accountId, long walletId, long depositRequestId)
        {
            new UserDepositDAC().CompletedByRequestId(accountId, walletId, depositRequestId);
        }

        public override void CancelByRequestId(Guid accountId, long walletId, long depositRequestId)
        {
            new UserDepositDAC().CancelByRequestId(accountId, walletId, depositRequestId);
        }
    }
}