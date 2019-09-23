using FiiiPay.Data;
using FiiiPay.DTO.Wallet;
using FiiiPay.Entities;
using System.Linq;
using System.Collections.Generic;
using System;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Foundation.Business.Agent;

namespace FiiiPay.Business
{
    public class UserWalletComponent : BaseComponent
    {
        public UserWallet GetUserWallet(Guid accountId, int coinId)
        {
            return new UserWalletDAC().GetUserWallet(accountId, coinId);
        }
        public ListForDepositOM ListForDepositAndWithdrawal(UserAccount user, string fiatCurrency)
        {
            try
            {
                var userWallets = new UserWalletDAC().GetUserWallets(user.Id);
                var coins = new CryptocurrencyDAC().GetAllActived().OrderBy(e => e.Sequence);
                //var rates = GetExchangeRates(user.CountryId, user.FiatCurrency);
                var priceList = new PriceInfoDAC().GetPrice(fiatCurrency);
                var list = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    decimal rate = 0;
                    if (userWallet != null)
                    {
                        rate = priceList.Where(t => t.CryptoID == userWallet.CryptoId).Select(t => t.Price).FirstOrDefault();
                    }
                    return GetDepositOMItemTempItem(userWallet, a, rate);
                })
                //.OrderBy(a => a.PayRank)
                .Select(a => new ListForDepositOMItem
                {
                    Id = a.Id,
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    FrozenBalance = a.FrozenBalance,
                    DecimalPlace = a.DecimalPlace,
                    IconUrl = a.IconUrl,
                    FiatBalance = a.FiatBalance,
                    UseableBalance = a.UseableBalance,
                    Name = a.Name,
                    CryptoEnable = a.CryptoEnable
                }).ToList();

                return new ListForDepositOM
                {
                    FiatCurrency = user.FiatCurrency,
                    List = list
                };
            }
            catch (Exception exception)
            {
                Error(exception.Message);
            }

            return null;
        }

        private ListForDepositOMItemTemp GetDepositOMItemTempItem(UserWallet model, Cryptocurrency coin, decimal rate)
        {
            model = model ?? new UserWallet { FrozenBalance = 0, Balance = 0, PayRank = int.MaxValue };
            return new ListForDepositOMItemTemp
            {
                Id = coin.Id,
                FrozenBalance = model.FrozenBalance.ToString(coin.DecimalPlace),
                IconUrl = coin.IconURL,
                Code = coin.Code,
                NewStatus = coin.Status ,
                UseableBalance = model.Balance.ToString(coin.DecimalPlace),
                PayRank = model.PayRank,
                FiatBalance = (model.Balance * rate).ToString(2),
                DecimalPlace = coin.DecimalPlace,
                Name = coin.Name,
                CryptoEnable = coin.Enable
            };
        }

        class ListForDepositOMItemTemp : ListForDepositOMItem
        {
            public int PayRank { get; set; }
        }

        public WalletPreReOrderOM PreReOrder(UserAccount user)
        {
            var userWallets = new UserWalletDAC().GetUserWallets(user.Id);
            var coins = new CryptocurrencyDAC().GetAll();
            var list = coins.Select(a =>
            {
                var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);

                return GetPreReOrderOMItemTempItem(userWallet, a);
            }).OrderBy(a => a.PayRank).Select(a => new WalletPreReOrderOMItem
            {
                Id = a.Id,
                Code = a.Code,
                IconUrl = a.IconUrl,
                Name = a.Name,
                Fixed = a.Fixed
            }).ToList();

            return new WalletPreReOrderOM
            {
                List = list
            };
        }

        private WalletPreReOrderOmItemTemp GetPreReOrderOMItemTempItem(UserWallet model, Cryptocurrency coin)
        {
            return new WalletPreReOrderOmItemTemp
            {
                Id = coin.Id,
                IconUrl = coin.IconURL,
                Code = coin.Code,
                PayRank = model == null ? int.MaxValue : model.PayRank,
                Name = coin.Name,
                Fixed = coin.Id == 1
            };
        }

        class WalletPreReOrderOmItemTemp : WalletPreReOrderOMItem
        {
            public int PayRank { get; set; }
        }

        public void ReOrder(UserAccount user, List<int> idList)
        {
            new UserWalletDAC().ReOrderForPay(user.Id, idList);
        }

        public UserWallet GenerateWallet(Guid accountId, string cryptoCode)
        {
            var cryptoDac = new CryptocurrencyDAC();
            var cryptocurrency = cryptoDac.GetByCode(cryptoCode);

            var UserWallet = new UserWallet
            {
                UserAccountId = accountId,
                CryptoId = cryptocurrency.Id,
                CryptoCode = cryptocurrency.Code,
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

        public UserWallet GenerateWallet(Guid accountId, int cryptoId)
        {
            var cryptoDac = new CryptocurrencyDAC();
            var cryptocurrency = cryptoDac.GetById(cryptoId);
            var UserWallet = new UserWallet
            {
                UserAccountId = accountId,
                CryptoId = cryptoId,
                CryptoCode = cryptocurrency.Code,
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
