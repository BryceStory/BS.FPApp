using FiiiPay.Foundation.Entities.Enum;
using System;
using System.Collections.Generic;

namespace FiiiPay.DTO.Order
{
    public class PrePayOM
    {
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 溢价费率，比如，0.1，客户端需要自行转换显示为10%
        /// </summary>
        public string MarkupRate { get; set; }

        public List<WalletItem> WaletList { get; set; }
    }

    public class WalletItem
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 图标的地址
        /// </summary>
        public Guid? IconUrl { get; set; }

        /// <summary>
        /// 货币各功能的状态
        /// </summary>
        public CryptoStatus NewStatus { get; set; }

        /// <summary>
        /// 货币的状态 0禁用 1可用，兼容旧版本
        /// </summary>
        public byte Status => NewStatus == 0 ? (byte)0 : (byte)1;

        /// <summary>
        /// 简称，比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称，比如：Bitcoin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public string UseableBalance { get; set; }

        /// <summary>
        /// 冻结的余额
        /// </summary>
        public string FrozenBalance { get; set; }

        /// <summary>
        /// 商家法币相对于该加密币的汇率，比如：要支付等值人民币的BTC，那么此值为0.00002013
        /// </summary>
        public string ExchangeRate { get; set; }

        /// <summary>
        /// 加密币换算成的法币额度
        /// </summary>
        public string FiatBalance { get; set; }

        /// <summary>
        /// 商家是否支持
        /// </summary>
        public bool MerchantSupported { get; set; }

        /// <summary>
        /// 小数点的位数
        /// </summary>
        public int DecimalPlace { get; set; }
        /// <summary>
        /// 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}
