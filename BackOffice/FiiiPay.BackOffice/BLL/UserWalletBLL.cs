using FiiiPay.Data;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class UserWalletBLL
    {
        public UserWallet GetUserWallet(Guid accountId, int coinId)
        {
            return new UserWalletDAC().GetUserWallet(accountId, coinId);
        }
        public UserWallet GenerateWallet(Guid accountId, int cryptoId)
        {
            var UserWallet = new UserWallet
            {
                UserAccountId = accountId,
                CryptoId = cryptoId,
                Balance = 0,
                FrozenBalance = 0,
                Address = null,
                Tag = null,
                HomePageRank = 0,
                PayRank = 0,
                ShowInHomePage = true
            };

            UserWallet.Id = new UserWalletDAC().Insert(UserWallet);

            return UserWallet;
        }
    }
}