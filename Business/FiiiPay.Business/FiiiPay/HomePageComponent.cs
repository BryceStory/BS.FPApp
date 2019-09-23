using FiiiPay.Data;
using FiiiPay.DTO.HomePage;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business
{
    public class HomePageComponent : BaseComponent
    {
        public IndexOM Index(UserAccount user)
        {
            try
            {
                var isBillerEnable = bool.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "BillerEnable").Value) ;
                var userWallets = new UserWalletDAC().GetUserWallets(user.Id);
                var currencyId = new CurrenciesDAC().GetByCode(user.FiatCurrency).ID;
                var exchangeRateList = new PriceInfoDAC().GetByCurrencyId(currencyId);
                var exchangeRate = exchangeRateList.ToDictionary(item => item.CryptoID);
                var coins = new CryptocurrencyDAC().GetAllActived().OrderBy(e => e.Sequence).ToList();
                var list = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    var rate = exchangeRate.ContainsKey(a.Id) ? exchangeRate[a.Id].Price : 0m;
                    return GetItem(userWallet, a, rate);
                }).Where(a => a.ShowInHomePage)
                //.OrderBy(a => a.HomePageRank)
                .Select(a => new CurrencyItem
                {
                    Id = a.Id,
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    FrozenBalance = a.FrozenBalance,
                    IconUrl = a.IconUrl,
                    UseableBalance = a.UseableBalance,
                    FiatBalance = a.FiatBalance,
                    FiatExchangeRate = exchangeRate.ContainsKey(a.Id) ? exchangeRate[a.Id].Price.ToString(4) : 0m.ToString(4),
                    CryptoEnable = a.CryptoEnable
            }).ToList();
                
                return new IndexOM
                {
                    TotalAmount = GetUserTotalAmount(user, userWallets, coins, exchangeRateList),
                    CurrencyItemList = list,
                    IsLV1Verified = user.L1VerifyStatus == VerifyStatus.Certified,
                    HasSetPin = !string.IsNullOrEmpty(user.Pin),
                    FiatCurrency = user.FiatCurrency,
                    IsBillerEnable = isBillerEnable
                };
            }
            catch (Exception exception)
            {
                Error(exception.StackTrace);
                return new IndexOM
                {
                    TotalAmount = "--",
                    CurrencyItemList = null,
                    FiatCurrency = user.FiatCurrency
                };
            }
        }

        private string GetUserTotalAmount(UserAccount user, List<UserWallet> userWallets, List<Cryptocurrency> coins, List<PriceInfo> priceInfoList)
        {
            var amount = userWallets.Sum(a =>
            {
                var coin = coins.FirstOrDefault(t => t.Id == a.CryptoId);
                if (coin == null)
                    return 0;
                var priceInfo = priceInfoList.Find(t => t.CryptoID == coin.Id);
                if (priceInfo == null) return 0;
                return (priceInfo.Price * (a.Balance + a.FrozenBalance).ToSpecificDecimal(coin.DecimalPlace));
            });

            return amount.ToString(2);
        }

        private CurrencyItemTemp GetItem(UserWallet model, Cryptocurrency coin, decimal rate)
        {
            model = model ?? new UserWallet { FrozenBalance = 0, Balance = 0, HomePageRank = int.MaxValue, ShowInHomePage = true };
            return new CurrencyItemTemp
            {
                Id = coin.Id,
                FrozenBalance = model.FrozenBalance.ToString(coin.DecimalPlace),
                IconUrl = coin.IconURL,
                Code = coin.Code,
                NewStatus = coin.Status,
                CryptoEnable = coin.Enable,
                UseableBalance = model.Balance.ToString(coin.DecimalPlace),
                FiatBalance = ((model.Balance + model.FrozenBalance )* rate).ToString(2),
                HomePageRank = model.HomePageRank,
                ShowInHomePage = model.ShowInHomePage
            };
        }

        public void ToggleShowInHomePage(UserAccount user, int coinId)
        {
            var wallet = new UserWalletDAC().GetByAccountId(user.Id, coinId)
                      ?? new UserWalletComponent().GenerateWallet(user.Id, coinId);
            new UserWalletDAC().UpdateShowInHomePage(wallet.Id, !wallet.ShowInHomePage);
        }

        class CurrencyItemTemp : CurrencyItem
        {
            public bool ShowInHomePage { get; set; }

            public int HomePageRank { get; set; }
        }

        public PreReOrder1OM PreReOrder(UserAccount user)
        {
            var userWallets = new UserWalletDAC().GetUserWallets(user.Id);
            var coins = new CryptocurrencyDAC().GetAllActived();
            var list = coins.Select(a =>
            {
                var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);

                return GetPreReOrderOMItemTempItem(userWallet, a);
            }).OrderByDescending(a => a.ShowInHomePage).ThenBy(a => a.HomePageRank).Select(a => new PreReOrder1OMItem
            {
                Id = a.Id,
                Code = a.Code,
                IconUrl = a.IconUrl,
                Name = a.Name,
                Fixed = a.Fixed,
                ShowInHomePage = a.ShowInHomePage
            }).ToList();

            return new PreReOrder1OM
            {
                List = list
            };
        }

        private PreReOrderOMItemTemp GetPreReOrderOMItemTempItem(UserWallet model, Cryptocurrency coin)
        {
            return new PreReOrderOMItemTemp
            {
                Id = coin.Id,
                IconUrl = coin.IconURL,
                Code = coin.Code,
                HomePageRank = model == null ? int.MaxValue : model.HomePageRank,
                Name = coin.Name,
                Fixed = coin.Id == 1,
                ShowInHomePage = model == null ? true : model.ShowInHomePage
            };
        }

        class PreReOrderOMItemTemp : PreReOrder1OMItem
        {
            public int HomePageRank { get; set; }
        }

        public void ReOrder(UserAccount user, List<int> idList)
        {
            new UserWalletDAC().ReOrderForHomePage(user.Id, idList);
        }
    }
}
