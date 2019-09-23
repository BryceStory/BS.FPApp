using System;

namespace FiiiPay.Entities.EntitySet
{
    public class OrderByPage
    {
        /// <summary>
		/// 
		/// </summary>
		public Guid Id { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Guid MerchantAccountId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Guid UserAccountId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int CryptoId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CryptoAmount { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ActualCryptoAmount { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal FiatAmount { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string FiatCurrency { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Markup { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ActualFiatAmount { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public byte Status { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Timestamp { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ExchangeRate { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpiredTime { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TransactionFee { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string MerchantIP { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string MerchantToken { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserIP { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserToken { set; get; }

        /// <summary>
        /// 1 - User scan merchant QR code 2 - Merchant scan user QR code 3 - User scan merchant Bluetooth 4 - Merchant scan user Bluetooth 5 - NFC
        /// </summary>
        public byte PaymentType { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime PaymentTime { set; get; }


        /// <summary>
        /// POST机编号
        /// </summary>
        public string PostSN { set; get; }

        /// <summary>
        /// 会员账号[手机号]
        /// </summary>
        public string Cellphone { set; get; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string CryptoName { set; get; }

        /// <summary>
        /// 返回条件的总记录数[分页用]
        /// </summary>
        public int TotalCount { set; get; }

        /// <summary>
        /// 当前汇率
        /// </summary>
        public string CurrentExchangeRate { set; get; }

        /// <summary>
        /// 汇率涨幅
        /// </summary>
        public string IncreaseRate { set; get; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string WithdrawalFee { set; get; }
        /// <summary>
        /// 手续费币种
        /// </summary>
        public int WithdrawalCryptoId { set; get; }

    }
}
