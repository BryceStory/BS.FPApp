namespace FiiiPOS.DTO
{
    /// <summary>
    /// The merchant withdrawal master setting information DTO
    /// </summary>
    public class MerchantWithdrawalMasterSettingDTO
    {
        /// <summary>
        /// 单次限额
        /// </summary>
        public decimal PerTxLimit { get; set; }
        /// <summary>
        /// 每日限额
        /// </summary>
        public decimal PerDayLimit { get; set; }
        /// <summary>
        /// 每月限额
        /// </summary>
        public decimal PerMonthLimit { get; set; }
        /// <summary>
        /// 提币到 场外 最小提币量
        /// </summary>
        public decimal ToOutsideMinAmount { get; set; }
        /// <summary>
        /// 提币到 FiiiPay 最小提币量
        /// </summary>
        public decimal ToUserMinAmount { get; set; }
        /// <summary>
        /// 提币到 FiiiPay 手续费率
        /// </summary>
        public decimal ToUserHandleFeeTier { get; set; }
        /// <summary>
        /// 提币到 FiiiPOS 手续费率
        /// </summary>
        public decimal ToMerchantHandleFeeTier { get; set; }


        /// <summary>
        /// 单次限额（一级认证）
        /// </summary>
        public decimal PerTxLimit1 { get; set; }
        /// <summary>
        /// 每日限额（一级认证）
        /// </summary>
        public decimal PerDayLimit1 { get; set; }
        /// <summary>
        /// 每月限额（一级认证）
        /// </summary>
        public decimal PerMonthLimit1 { get; set; }
        /// <summary>
        /// 单次限额（二级认证）
        /// </summary>
        public decimal PerTxLimit2 { get; set; }
        /// <summary>
        /// 每日限额（二级认证）
        /// </summary>
        public decimal PerDayLimit2 { get; set; }
        /// <summary>
        /// 每月限额（二级认证）
        /// </summary>
        public decimal PerMonthLimit2 { get; set; }
    }
}