using System;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;

namespace FiiiPOS.Business.FiiiPOS
{
    public partial class OrderComponent
    {
        private Order Generate(MerchantAccount account, Cryptocurrency coin, string unifiedFiatCurrency, decimal amount, PaymentType paymentType, string merchantClientIP = null)
        {
            return Generate(account, coin, unifiedFiatCurrency, account.FiatCurrency, amount, paymentType, merchantClientIP);
        }

        private Order Generate(MerchantAccount account, Cryptocurrency coin, string unifiedFiatCurrency, string fiatCurrency, decimal amount, PaymentType paymentType, string merchantClientIP = null)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                MerchantAccountId = account.Id,
                CryptoId = coin.Id,
                CryptoCode = coin.Code,
                FiatAmount = amount,
                PaymentType = paymentType,
                FiatCurrency = fiatCurrency,
                Status = OrderStatus.Pending,
                Timestamp = DateTime.UtcNow,
                ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                Markup = account.Markup,
                ExchangeRate = GetExchangeRate(account.CountryId, fiatCurrency, coin),
                UnifiedExchangeRate = GetExchangeRate(account.CountryId, unifiedFiatCurrency, coin),
                UnifiedFiatCurrency = unifiedFiatCurrency,
                MerchantIP = merchantClientIP
            };

            var calcModel =
                CalculateAmount(amount, account.Markup, account.Receivables_Tier, order.ExchangeRate, coin);

            order.ActualFiatAmount = calcModel.FiatTotalAmount;
            order.CryptoAmount = calcModel.CryptoAmount;
            order.TransactionFee = calcModel.TransactionFee;
            order.ActualCryptoAmount = calcModel.ActualCryptoAmount;

            var model = CalculateUnifiedAmount(order.CryptoAmount, order.ActualCryptoAmount, order.UnifiedExchangeRate);
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
            //总金额 = 消费金额 + 消费金额 x 溢价
            decimal fiatTotalAmount = (fiatAmount * (1 + markup)).ToSpecificDecimal(2);
            //手续费 = 消费金额 x 手续费率
            decimal transactionFee = ((fiatAmount * feeRate) / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);

            //总数量
            decimal cryptoAmount = (fiatTotalAmount / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);
            //实收数量 = 总数量 - 手续费
            decimal actualCryptoAmount = cryptoAmount - transactionFee;

            return new CalcModel(fiatTotalAmount, cryptoAmount, transactionFee, actualCryptoAmount);
        }

        /// <summary>
        /// 根据支付的加密币数量，计算法币的数量
        /// </summary>
        /// <param name="cryptoTotalAmount">加密币总数量</param>
        /// <param name="cryptoAmount">加密币实际支付数量</param>
        /// <param name="exchangeRate">加密币对法币汇率</param>
        /// <returns></returns>
        private UnifiedModel CalculateUnifiedAmount(decimal cryptoTotalAmount, decimal cryptoAmount , decimal exchangeRate)
        {
            decimal unifiedAmount = (cryptoTotalAmount * exchangeRate).ToSpecificDecimal(2);
            decimal unifiedActualAmount = (cryptoAmount * exchangeRate).ToSpecificDecimal(2);

            return new UnifiedModel(unifiedAmount, unifiedActualAmount);
        }
    }

    public class UnifiedModel
    {
        public decimal UnifiedFiatAmount { get; set; }

        public decimal UnifiedActualFiatAmount { get; set; }

        public UnifiedModel(decimal unifiedFiatAmount, decimal unifiedActualFiatAmount)
        {
            this.UnifiedFiatAmount = unifiedFiatAmount;
            this.UnifiedActualFiatAmount = unifiedActualFiatAmount;
        }
    }

    public class CalcModel
    {
        public  decimal FiatTotalAmount { get; set; }

        public decimal CryptoAmount { get; set; }

        public decimal TransactionFee { get; set; }

        public decimal ActualCryptoAmount { get; set; }

        public CalcModel(decimal fiatTotalAmount, decimal cryptoAmount, decimal transactionFee, decimal actualCryptoAmount)
        {
            this.FiatTotalAmount = fiatTotalAmount;
            this.CryptoAmount = cryptoAmount;
            this.TransactionFee = transactionFee;
            this.ActualCryptoAmount = actualCryptoAmount;
        }
    }
}