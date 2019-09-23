using System;

namespace FiiiPay.DTO.Transfer
{
    /// <summary>
    /// 转账准备返回
    /// </summary>
    public class PreTransferOM
    {
        /// <summary>
        /// 目标用户头像
        /// </summary>
        public Guid ToAvatar { get; set; }
        /// <summary>
        /// 目标用户名
        /// </summary>
        public string ToAccountName { get; set; }
        /// <summary>
        /// 目标姓名
        /// </summary>
        public string ToFullname { get; set; }
        /// <summary>
        /// 是否允许转账
        /// </summary>
        public bool IsTransferAbled { get; set; }
        /// <summary>
        /// 是否已实名认证
        /// </summary>
        public bool IsProfileVerified { get; set; }
        /// <summary>
        /// 转账加密币币种ID
        /// </summary>
        public int CoinId { get; set; }
        /// <summary>
        /// 转账加密币币种编码
        /// </summary>
        public string CoinCode { get; set; }
        /// <summary>
        /// 加密币最小转账金额
        /// </summary>
        public string MinCount { get; set; }
        /// <summary>
        /// 加密币小数最大位数
        /// </summary>
        public string CoinDecimalPlace { get; set; }
        /// <summary>
        /// 加密币可用余额
        /// </summary>
        public string CoinBalance { get; set; }
        /// <summary>
        /// 法币编码
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 加密币兑法币汇率
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string ChargeFee { get; set; }
    }
}
