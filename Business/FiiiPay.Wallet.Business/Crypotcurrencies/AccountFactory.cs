using System;
using FiiiPay.Data;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class AccountFactory
    {
        public abstract Account GetById(Guid id);
    }

    public class MerchantAccountFactory : AccountFactory
    {
        public override Account GetById(Guid id)
        {
            var baseAccount = new MerchantAccountDAC().GetById(id);
            if (baseAccount == null)
                return null;
            return new Account
            {
                Id = baseAccount.Id,
                Email = baseAccount.Email
            };
        }
    }

    public class UserAccountFactory : AccountFactory
    {
        public override Account GetById(Guid id)
        {
            var baseAccount = new UserAccountDAC().GetById(id);
            if (baseAccount == null)
                return null;
            return new Account
            {
                Id = baseAccount.Id,
                Email = baseAccount.Email
            };
        }
    }
}