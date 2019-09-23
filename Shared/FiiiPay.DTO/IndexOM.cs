using FiiiPay.Foundation.Entities.Enum;
using System;
using System.Collections.Generic;

namespace FiiiPay.DTO.HomePage
{
    public class IndexOM
    {
        /// <summary>
        /// 所有加密币换算成当地币种的总金额
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 是否已经设置Pin
        /// </summary>
        public bool HasSetPin { get; set; }

        /// <summary>
        /// 是否通过了KYC审核
        /// </summary>
        public bool IsLV1Verified { get; set; }
        /// <summary>
        /// 生活缴费是否可用
        /// </summary>
        public bool IsBillerEnable { get; set; }

        public List<CurrencyItem> CurrencyItemList { get; set; }
    }

    public class CurrencyItem
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 图标的地址，客户端需要拼接地址下载图片
        /// </summary>
        public Guid? IconUrl { get; set; }

        /// <summary>
        /// 货币状态 0禁用 1可用 2禁止充提币
        /// </summary>
        public CryptoStatus NewStatus { get; set; }

        /// <summary>
        /// 币的简称，比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public string UseableBalance { get; set; }

        /// <summary>
        /// 冻结的余额
        /// </summary>
        public string FrozenBalance { get; set; }

        /// <summary>
        /// 法币兑换率
        /// </summary>
        public string FiatExchangeRate { get; set; }
        /// <summary>
        /// 加密币换算成的法币额度
        /// </summary>
        public string FiatBalance { get; set; }
        /// <summary>
        /// 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }

        public byte Status => NewStatus == 0 ? (byte) 0 : (byte) 1;
    }
}
