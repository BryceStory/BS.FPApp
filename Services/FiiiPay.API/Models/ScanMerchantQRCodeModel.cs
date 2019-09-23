using System;
using System.Collections.Generic;
using FiiiPay.Foundation.Entities.Enum;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// 扫描商家固态二维码返回信息
    /// </summary>
    public class ScanMerchantQRCodeModel
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public Guid MerchantId { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 商家头像ID
        /// </summary>
        public Guid? Avatar { get; set; }
        /// <summary>
        /// 商家LV1认证状态
        /// </summary>
        public byte L1VerifyStatus { get; set; }
        /// <summary>
        /// 商家LV2认证状态
        /// </summary>
        public byte L2VerifyStatus { get; set; }
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 溢价费率，比如，0.1，客户端需要自行转换显示为10%
        /// </summary>
        public string MarkupRate { get; set; }

        /// <summary>
        /// 加密币钱包详细列表
        /// </summary>
        public List<WalletInfo> WaletInfoList { get; set; }
    }
    /// <summary>
    /// 加密币钱包详情
    /// </summary>
    public class WalletInfo
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
        /// 货币的状态 0禁用 1可用
        /// </summary>
        public CryptoStatus NewStatus { get; set; }

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
        /// 0禁用 1启用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}