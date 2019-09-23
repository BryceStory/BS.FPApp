using FiiiPay.Foundation.Entities.Enum;
using System;
using System.Collections.Generic;

namespace FiiiPay.DTO.Wallet
{
    public class ListForDepositOM
    {
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FiatCurrency { get; set; }
        public List<ListForDepositOMItem> List { get; set; }
    }

    public class ListForDepositOMItem
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
        /// 币的简称，比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 货币的状态 0禁用 1可用
        /// </summary>
        public CryptoStatus NewStatus { get; set; }

        public byte Status => NewStatus == 0 ? (byte)0 : (byte)1;

        /// <summary>
        /// 币的名称，比如：Bitcoin
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
        /// 加密币换算成的法币额度
        /// </summary>
        public string FiatBalance { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int DecimalPlace { get; set; }
        /// <summary>
        /// 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}
