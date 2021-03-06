﻿using FiiiPay.Data;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.MiningConfirmService.Factories
{
    public abstract class WalletStatementFactory
    {
        public abstract void Insert(WalletStatement statement);
    }


    public class MerchantWalletStatementFactory : WalletStatementFactory
    {
        public override void Insert(WalletStatement statement)
        {
            var merchantWalletStatement = new MerchantWalletStatement
            {
                Action = statement.Action,
                Amount = statement.Amount,
                Balance = statement.Balance,
                Remark = statement.Remark,
                Timestamp = statement.Timestamp,
                WalletId = statement.WalletId
            };
            new MerchantWalletStatementDAC().Insert(merchantWalletStatement);
        }
    }

    public class UserWalletStatementFactory : WalletStatementFactory
    {
        public override void Insert(WalletStatement statement)
        {
            var userWalletStatement = new UserWalletStatement
            {
                Action = statement.Action,
                Amount = statement.Amount,
                Balance = statement.Balance,
                FrozenAmount = statement.FrozenAmount,
                FrozenBalance = statement.FrozenBalance,
                Remark = statement.Remark,
                Timestamp = statement.Timestamp,
                WalletId = statement.WalletId
            };
            new UserWalletStatementDAC().Insert(userWalletStatement);
        }
    }
}
