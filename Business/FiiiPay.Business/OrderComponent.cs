using System;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;

namespace FiiiPay.Business
{
    public partial class OrderComponent
    {
        private Order Generate(MerchantAccount merchantAccount, Cryptocurrency coin, string unifiedFiatCurrency, decimal fiatAmount, PaymentType paymentType, string merchantClientIP = null)
        {
            return Generate(merchantAccount, coin, unifiedFiatCurrency, merchantAccount.FiatCurrency, fiatAmount, paymentType, merchantClientIP);
        }

        private Order Generate(MerchantAccount merchantAccount, Cryptocurrency coin, string unifiedFiatCurrency, string fiatCurrency, decimal fiatAmount, PaymentType paymentType, string merchantClientIP = null)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = IdentityHelper.OrderNo(),
                MerchantAccountId = merchantAccount.Id,
                CryptoId = coin.Id,
                CryptoCode = coin.Code,
                FiatAmount = fiatAmount,
                PaymentType = paymentType,
                FiatCurrency = fiatCurrency,
                Status = OrderStatus.Pending,
                Timestamp = DateTime.UtcNow,
                ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                Markup = merchantAccount.Markup,
                ExchangeRate = GetExchangeRate(merchantAccount.CountryId, fiatCurrency, coin),
                UnifiedExchangeRate = GetExchangeRate(merchantAccount.CountryId, unifiedFiatCurrency, coin),
                UnifiedFiatCurrency = unifiedFiatCurrency,
                MerchantIP = merchantClientIP
            };

            //(order.ActualFiatAmount, order.CryptoAmount, order.TransactionFee, order.ActualCryptoAmount) =
            //    CalculateAmount(amount, account.Markup, account.Receivables_Tier, order.ExchangeRate, coin);
            //(order.UnifiedFiatAmount, order.UnifiedActualFiatAmount) = CalculateUnifiedAmount(order.FiatAmount, order.ActualFiatAmount, order.UnifiedExchangeRate);
            //return order;
            var calcModel =
                CalculateAmount(fiatAmount, merchantAccount.Markup, merchantAccount.Receivables_Tier, order.ExchangeRate, coin);

            order.ActualFiatAmount = calcModel.FiatTotalAmount;
            order.CryptoAmount = calcModel.CryptoAmount;
            order.TransactionFee = calcModel.TransactionFee;
            order.ActualCryptoAmount = calcModel.ActualCryptoAmount;

            var model = CalculateUnifiedAmount(calcModel.CryptoAmount, calcModel.ActualCryptoAmount, order.UnifiedExchangeRate);
            order.UnifiedFiatAmount = model.UnifiedFiatAmount;
            order.UnifiedActualFiatAmount = model.UnifiedActualFiatAmount;
            return order;
        }

        /// <summary>
        /// 计算订单金额
        /// </summary>
        /// <param name="fiatAmount">消费金额</param>
        /// <param name="markup">溢价率</param>
        /// <param name="feeRate">手续费率</param>
        /// <param name="exchangeRate">法比对加密币汇率</param>
        /// <param name="coin">加密币实体</param>
        /// <returns>(法币总金额,加密币总数量,加密币手续费,加密币实收数量)</returns>
        private CalcModel CalculateAmount(decimal fiatAmount, decimal markup, decimal feeRate, decimal exchangeRate, Cryptocurrency coin)
        {
            //var tempFiatAmount = (fiatAmount * (1 + markup));
            var tempFiatAmount = fiatAmount + (fiatAmount * markup).ToSpecificDecimal(4);

            //总金额 = 消费金额 + 消费金额 x 溢价
            //decimal fiatTotalAmount = (fiatAmount * (1 + markup)).ToSpecificDecimal(4);
            decimal fiatTotalAmount = tempFiatAmount;
            //手续费 = 消费金额 x 手续费率
            decimal transactionFee = ((fiatAmount * feeRate) / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);

            //总数量
            decimal cryptoAmount = ((fiatAmount + fiatAmount * markup) / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);
            //实收数量 = 总数量 - 手续费
            decimal actualCryptoAmount = cryptoAmount - transactionFee;

            return new CalcModel(fiatTotalAmount, cryptoAmount, transactionFee, actualCryptoAmount);
        }

        /// <summary>
        /// 根据支付的加密币数量，计算法币的数量
        /// </summary>
        /// <param name="cryptoAmount">加密币总数量</param>
        /// <param name="actualCryptoAmount">加密币实际支付数量（扣除手续费）</param>
        /// <param name="exchangeRate">加密币对法币汇率</param>
        /// <returns></returns>
        private UnifiedModel CalculateUnifiedAmount(decimal cryptoAmount, decimal actualCryptoAmount, decimal exchangeRate)
        {
            decimal unifiedFiatAmount = (cryptoAmount * exchangeRate).ToSpecificDecimal(4);
            decimal unifiedFiatActualAmount = (actualCryptoAmount * exchangeRate).ToSpecificDecimal(4);

            return new UnifiedModel(unifiedFiatAmount, unifiedFiatActualAmount);
        }

        private decimal GetExchangeRate(int countryId, string fiatCurrency, Cryptocurrency crypto)
        {
            var agent = new MarketPriceComponent();
            var price = agent.GetMarketPrice(fiatCurrency, crypto.Code);
            if (price == null) return 0M;
            return price.Price;
        }
    }
    
    public class UnifiedModel
    {
        public decimal UnifiedFiatAmount { get; set; }

        public decimal UnifiedActualFiatAmount { get; set; }

        public UnifiedModel(decimal unifiedFiatAmount, decimal unifiedActualFiatAmount)
        {
            UnifiedFiatAmount = unifiedFiatAmount;
            UnifiedActualFiatAmount = unifiedActualFiatAmount;
        }
    }

    public class CalcModel
    {
        public decimal FiatTotalAmount { get; set; }

        public decimal CryptoAmount { get; set; }

        public decimal TransactionFee { get; set; }

        public decimal ActualCryptoAmount { get; set; }

        public CalcModel(decimal fiatTotalAmount, decimal cryptoAmount, decimal transactionFee, decimal actualCryptoAmount)
        {
            FiatTotalAmount = fiatTotalAmount;
            CryptoAmount = cryptoAmount;
            TransactionFee = transactionFee;
            ActualCryptoAmount = actualCryptoAmount;
        }
    }
}