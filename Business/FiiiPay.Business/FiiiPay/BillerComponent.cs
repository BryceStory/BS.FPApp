using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO;
using FiiiPay.Entities;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.Business.FiiiPay
{
    public class BillerComponent
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BillerComponent));
        public BillerPayOM Pay(UserAccount account, BillerPayIM im, ref int errorCode)
        {
            var coin = new CryptocurrencyDAC().GetById(im.CryptoId);
            if (!coin.Status.HasFlag(CryptoStatus.Biller) || coin.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);
            var result = PrePay(new BillerPrePayIM()
            {
                CountryId = im.CountryId, CryptoAmount = im.CryptoAmount, CryptoCode = im.CryptoCode,
                CryptoId = im.CryptoId, FiatAmount = im.FiatAmount, FiatCurrency = im.FiatCurrency
            }).Status;
            if (result != 0)
            {
                errorCode = 10400 + result;
                return null;
            }

            new SecurityComponent().VerifyPin(account, im.Pin);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(im.CryptoId);
            if (cryptoCurrency?.Id == null)
                throw new CommonException(ReasonCode.CRYPTO_NOT_EXISTS, "Error: Invalid Cryptocurrency");
            var uwComponent = new UserWalletComponent();
            var userWallet = uwComponent.GetUserWallet(account.Id, im.CryptoId);
            if (userWallet == null)
            {
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            }
            if (userWallet.Balance < im.CryptoAmount)
            {
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            }
            var uwDAC = new UserWalletDAC();
            var uwsDAC = new UserWalletStatementDAC();
            var boDAC = new BillerOrderDAC();
            var billerOrder = new BillerOrder()
            {
                Id = Guid.NewGuid(),
                BillerCode = im.BillerCode,
                CryptoAmount = im.CryptoAmount,
                CryptoCode = im.CryptoCode,
                CryptoId = im.CryptoId,
                Discount = im.CryptoCode.Equals("Fiii", StringComparison.InvariantCultureIgnoreCase)
                    ? decimal.Parse(new MasterSettingDAC().SelectByGroup("BillerMaxAmount").First(item =>
                        item.Name.Equals("DiscountRate", StringComparison.CurrentCultureIgnoreCase)).Value)
                    : 0,
                ExchangeRate = im.ExchangeRate,
                FiatAmount = im.FiatAmount,
                FiatCurrency = im.FiatCurrency,
                ReferenceNumber = im.ReferenceNumber,
                Tag = im.Tag,
                Status = BillerOrderStatus.Pending,
                Timestamp = DateTime.UtcNow,
                AccountId = account.Id,
                OrderNo = IdentityHelper.OrderNo(),
                PayTime = DateTime.UtcNow,
                CountryId = im.CountryId
            };
            var address = new BillerAddressDAC().GetAllAddresses(account.Id).FirstOrDefault(item =>
                item.BillerCode == im.BillerCode && im.ReferenceNumber == item.ReferenceNumber);

            billerOrder.Tag = address?.Tag;
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                uwDAC.Decrease(userWallet.Id, billerOrder.CryptoAmount);
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = UserWalletStatementAction.Consume,
                    Amount = -billerOrder.CryptoAmount,
                    Balance = userWallet.Balance - billerOrder.CryptoAmount,
                    FrozenAmount = 0,
                    FrozenBalance = userWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });

                boDAC.Insert(billerOrder);
                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = billerOrder.AccountId,
                    CryptoId = cryptoCurrency.Id,
                    CryptoCode = cryptoCurrency.Code,
                    Type = UserTransactionType.BillOrder,
                    DetailId = billerOrder.Id.ToString(),
                    Status = (byte)billerOrder.Status,
                    Timestamp = billerOrder.Timestamp,
                    Amount = billerOrder.CryptoAmount,
                    OrderNo = billerOrder.OrderNo
                });
                scope.Complete();
            }

            return new BillerPayOM()
            {
                CryptoAmount = billerOrder.CryptoAmount.ToString(), OrderNo = billerOrder.OrderNo,
                CryptoCode = billerOrder.CryptoCode, OrderId = billerOrder.Id,
                Timestamp = billerOrder.PayTime.ToUnixTime().ToString(),
                SaveAddress = address != null
            };
        }

        public BillerPrePayOM PrePay(BillerPrePayIM im)
        {
            var exchangeRate = new PriceInfoDAC()
                .GetPriceInfo(new CurrenciesDAC().GetByCode(im.FiatCurrency).ID, im.CryptoId).Price;
            var discount = im.CryptoCode.Equals("Fiii", StringComparison.InvariantCultureIgnoreCase)
                ? 1 - decimal.Parse(new MasterSettingDAC().SelectByGroup("BillerMaxAmount").First(item =>
                      item.Name.Equals("DiscountRate", StringComparison.CurrentCultureIgnoreCase)).Value)
                : 1;
            var errorTolerantRate =
                decimal.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "Error_Tolerant_Rate").Value); 
           
            if (!(im.CryptoAmount <= ((im.FiatAmount / exchangeRate) * discount) * (1+errorTolerantRate) && im.CryptoAmount >= ((im.FiatAmount / exchangeRate) * discount) * (1 - errorTolerantRate)))
            {
                throw new CommonException(ReasonCode.BillerInvalidValues,Resources.BillerExchangeRateError);
            }

            var dac = new BillerOrderDAC();
            var maxValue = decimal.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "Biller_MaxAmount ").Value);
            if (im.FiatAmount > maxValue)
            {
                throw new CommonException(ReasonCode.BillerOverMaxAmount, string.Format(Resources.BillerOverMaxAmount, $"{maxValue} {im.FiatCurrency}"));
            }
           
            var totalMonthAmount = dac.GetMonthAmount(DateTime.UtcNow);
            
            if (totalMonthAmount + im.FiatAmount >
                decimal.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "Biller_Month_MaxAmount").Value))
            {
                throw new CommonException(ReasonCode.BillerOverMonthMaxAmount, Resources.BillerOverMonthMaxAmount);
            }
            var totalDayAmount = dac.GetDayAmount(DateTime.UtcNow);

            if (totalDayAmount + im.FiatAmount >
                decimal.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "Biller_Day_MaxAmount").Value))
            {
                throw new CommonException(ReasonCode.BillerOverDayMaxAmount, Resources.BillerOverDayMaxAmount);
            }

            if (new CountryComponent().GetById(im.CountryId) == null)
            {
                return new BillerPrePayOM() { Status = 5 };
            }
            return new BillerPrePayOM() {Status = 0};
        }
        
        public BillerDetailOM Detail(UserAccount account, Guid id)
        {
            var order = new BillerOrderDAC().GetById(id);
            var currentRate = new CryptocurrencyAgent()
                .GetMarketPrice(account.CountryId, order.FiatCurrency, order.CryptoCode).Price;
            return new BillerDetailOM()
            {
                CryptoCode = order.CryptoCode,
                Id = order.Id,
                CurrentExchangeRate = $"1 {order.CryptoCode} = {currentRate.ToString(4)} {order.FiatCurrency}",
                BillerCode = order.BillerCode,
                Status = order.Status,
                Tag = order.Tag,
                ReferenceNumber = order.ReferenceNumber,
                FiatAmount = order.FiatAmount.ToString(2),
                FiatCurrency = order.FiatCurrency,
                ExchangeRate = $"1 {order.CryptoCode} = {order.ExchangeRate.ToString(4)} {order.FiatCurrency}",
                IncreaseRate = ((currentRate - order.ExchangeRate)/order.ExchangeRate) > 0 ? $"+{((currentRate - order.ExchangeRate) / order.ExchangeRate*100).ToString(2)}" : ((currentRate - order.ExchangeRate) / order.ExchangeRate*100).ToString(2),
                CryptoAmount = order.CryptoAmount.ToString(CultureInfo.InvariantCulture),
                Timestamp = order.PayTime.ToUnixTime().ToString(),
                OrderNo = order.OrderNo,
                Remark = order.Remark,
                StatusStr = order.Status == BillerOrderStatus.Complete ? Resources.BillerOrderComplete : order.Status == BillerOrderStatus.Fail ? Resources.BillerOrderFail : Resources.BillerOrderPending,
                TypeStr = Resources.BillerDetailType
            };
        }

        public void AddAddress(UserAccount account, BillerAddAddressIM im)
        {
            var count = new BillerAddressDAC().GetAllAddresses(account.Id).Count;
            if (count >= 500)
            {
                throw new CommonException(ReasonCode.BillerOverMaxAddressCount, Resources.BillerOverAddressMaxCount);
            }

            if (new BillerAddressDAC().GetAllAddresses(account.Id).Any(item =>
                item.BillerCode == im.BillerCode && im.ReferenceNumber == item.ReferenceNumber))
            {
                throw new CommonException(ReasonCode.BillerAddressExisted, Resources.BillerAddressExisted);
            }
            var address = new BillerAddress()
            {
                BillerCode = im.BillerCode,
                IconIndex = im.IconIndex,
                ReferenceNumber = im.ReferenceNumber,
                Tag = im.Tag,
                AccountId = account.Id,
                Timestamp = DateTime.UtcNow
            };
            new BillerAddressDAC().Insert(address);
        }

        public void DeleteAddress(Guid accountId, long id)
        {
            if (new BillerAddressDAC().GetAllAddresses(accountId).All(item => item.Id != id))
            {
                throw new Exception();
            }
            new BillerAddressDAC().Delete(id);
        }

        public void EditAddress(UserAccount account, BillerEditAddressIM im)
        {
            if (new BillerAddressDAC().GetAllAddresses(account.Id).Any(item =>
                item.BillerCode == im.BillerCode && im.ReferenceNumber == item.ReferenceNumber && item.Id != im.Id))
            {
                throw new CommonException(ReasonCode.BillerAddressExisted, Resources.BillerAddressExisted);
            }
            new BillerAddressDAC().Update(new BillerAddress()
            {
                BillerCode = im.BillerCode,
                IconIndex = im.IconIndex,
                Id = im.Id,
                ReferenceNumber = im.ReferenceNumber,
                Tag = im.Tag,
                Timestamp = DateTime.UtcNow
            });
        }

        public List<BillerAddressOM> GetAddresses(UserAccount account, BillerGetAddressIM im)
        {
            if (string.IsNullOrEmpty(im.IconIndex))
            {
                return new BillerAddressDAC().GetAddresses(account.Id, im.PageSize, im.PageIndex).Select(item =>
                    new BillerAddressOM()
                    {
                        BillerCode = item.BillerCode,
                        IconIndex = item.IconIndex,
                        Id = item.Id,
                        ReferenceNumber = item.ReferenceNumber,
                        Tag = item.Tag,
                        Timestamp = item.Timestamp.ToUnixTime().ToString()
                    }).ToList();
            }

            return new BillerAddressDAC().GetAddressesByIconIndex(account.Id, im.IconIndex, im.PageSize, im.PageIndex)
                .Select(item => new BillerAddressOM()
                {
                    BillerCode = item.BillerCode,
                    IconIndex = item.IconIndex,
                    Id = item.Id,
                    ReferenceNumber = item.ReferenceNumber,
                    Tag = item.Tag,
                    Timestamp = item.Timestamp.ToUnixTime().ToString()
                }).ToList();
        }

        public IEnumerable<BillerCryptoItemOM> GetBillerCryptoCurrency(UserAccount user, string fiatCurrency)
        {
            
            var dac = new MasterSettingDAC();
            var discount = dac.SelectByGroup("BillerMaxAmount").First(item =>
                item.Name.Equals("DiscountRate", StringComparison.CurrentCultureIgnoreCase));

            var coins = new UserWalletComponent().ListForDepositAndWithdrawal(user, fiatCurrency).List;

            var priceInfoDict = new PriceInfoDAC().GetByCurrencyId(new CurrenciesDAC().GetByCode(fiatCurrency).ID).ToDictionary(item => item.CryptoID);

            foreach (var item in coins)
            {
                var model = new BillerCryptoItemOM()
                {
                    Code = item.Code,
                    Id = item.Id,
                    Name = item.Name,
                    CryptoEnable = item.CryptoEnable,
                    DecimalPlace = item.DecimalPlace,
                    FiatBalance = item.FiatBalance,
                    FrozenBalance = item.FrozenBalance,
                    IconUrl = item.IconUrl,
                    NewStatus = item.NewStatus,
                    UseableBalance = item.UseableBalance,
                    ExchangeRate = priceInfoDict.ContainsKey(item.Id) ? priceInfoDict[item.Id].Price.ToString(4) : 0.ToString()
                };
                if (item.Code.Equals("fiii", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.Discount = decimal.Parse(discount.Value).ToString();
                    yield return model;
                }
                else
                {
                    model.Discount = "0";
                    yield return model;
                }
            }
        }

        public BillerMessageFailOM MessageFail(Guid id)
        {
            var order = new BillerOrderDAC().GetById(id);

            return new BillerMessageFailOM()
            {
                Content = order.Remark,
                Timestamp = order.FinishTime.ToUnixTime().ToString(),
                Title = Resources.BillerMessageFailTitle
            };
        }

        public bool IsBillerForbbiden()
        {
            return bool.Parse(new MasterSettingDAC().Single("BillerMaxAmount", "BillerEnable").Value);
        }
    }
}
