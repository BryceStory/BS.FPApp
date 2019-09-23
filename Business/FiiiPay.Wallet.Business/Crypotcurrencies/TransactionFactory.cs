using FiiiPay.Data;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class TransactionFactory
    {
        public abstract void Insert(UserTransaction tran);
        public abstract void UpdateStatus(UserTransactionType userTransactionType, string detailId,Guid accountId, byte transactionStatus);
    }

    public class UserTransactionFactory : TransactionFactory
    {
        public override void Insert(UserTransaction tran)
        {
            new UserTransactionDAC().Insert(tran);
        }

        public override void UpdateStatus(UserTransactionType userTransactionType, string detailId, Guid accountId, byte transactionStatus)
        {
            new UserTransactionDAC().UpdateStatus(userTransactionType, detailId, accountId, transactionStatus);
        }
    }
    public class MerchantTransactionFactory : TransactionFactory
    {
        public override void Insert(UserTransaction tran)
        {
            return;
        }

        public override void UpdateStatus(UserTransactionType userTransactionType, string detailId, Guid accountId, byte transactionStatus)
        {
            return;
        }
    }
}
