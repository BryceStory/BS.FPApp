using System;
using FiiiPay.Data;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class WithdrawFactory
    {
        public abstract Withdraw GetByRequestId(Guid accountId, long walletId, long withdrawRequestId);
        public abstract void CompletedByRequestId(Guid accountId, long walletId, long withdrawRequestId,string transactionId);

        public abstract void RejectByRequestId(Guid accountId, long walletId, long withdrawRequestId, string reason);
        public abstract void InitTransactionId(Guid accountId, long walletId, long withdrawRequestId, string withdrawTransactionId);
    }

    public class MerchantWithdrawFactory : WithdrawFactory
    {
        public override Withdraw GetByRequestId(Guid accountId, long walletId, long withdrawRequestId)
        {
            var baseWithdraw = new MerchantWithdrawalDAC().GetByRequestId(accountId, walletId, withdrawRequestId);
            if (baseWithdraw == null)
                return null;
            return new Withdraw
            {
                Id = baseWithdraw.Id,
                Amount = baseWithdraw.Amount,
                Status = baseWithdraw.Status,
                TransactionId = baseWithdraw.TransactionId
            };
        }

        public override void CompletedByRequestId(Guid accountId, long walletId, long withdrawRequestId, string transactionId)
        {
            if(!string.IsNullOrWhiteSpace(transactionId))
                new MerchantWithdrawalDAC().CompletedByRequestId(accountId, walletId, withdrawRequestId, transactionId);
            else
            {
                new MerchantWithdrawalDAC().CompletedByRequestId(accountId, walletId, withdrawRequestId);
            }
        }

        public override void RejectByRequestId(Guid accountId, long walletId, long withdrawRequestId, string reason)
        {
            new MerchantWithdrawalDAC().RejectByRequestId(accountId, walletId, withdrawRequestId, reason);
        }

        public override void InitTransactionId(Guid accountId, long walletId, long withdrawRequestId, string withdrawTransactionId)
        {
            new MerchantWithdrawalDAC().InitTransactionId(accountId, walletId, withdrawRequestId, withdrawTransactionId);
        }
    }

    public class UserWithdrawFactory : WithdrawFactory
    {
        public override Withdraw GetByRequestId(Guid accountId, long walletId, long withdrawRequestId)
        {
            var baseWithdraw = new UserWithdrawalDAC().GetByRequestId(accountId, walletId, withdrawRequestId);
            if (baseWithdraw == null)
                return null;
            return new Withdraw
            {
                Id = baseWithdraw.Id,
                Amount = baseWithdraw.Amount,
                Status = baseWithdraw.Status,
                TransactionId = baseWithdraw.TransactionId
            };
        }

        public override void CompletedByRequestId(Guid accountId, long walletId, long withdrawRequestId,string transactionId)
        {
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                new UserWithdrawalDAC().CompletedByRequestId(accountId, walletId, withdrawRequestId, transactionId);
            }
            else
            {
                new UserWithdrawalDAC().CompletedByRequestId(accountId, walletId, withdrawRequestId);
            }
        }

        public override void RejectByRequestId(Guid accountId, long walletId, long withdrawRequestId, string reason)
        {
            new UserWithdrawalDAC().RejectByRequestId(accountId, walletId, withdrawRequestId, reason);
        }

        public override void InitTransactionId(Guid accountId, long walletId, long withdrawRequestId, string transactionId)
        {
            new UserWithdrawalDAC().InitTransactionId(accountId, walletId, withdrawRequestId, transactionId);
        }
    }
}