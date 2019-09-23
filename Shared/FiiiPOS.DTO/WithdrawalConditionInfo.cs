namespace FiiiPOS.DTO
{
    public class WithdrawalConditionInfo
    {
        /// <summary>
        /// 可用余额
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// 单次限额
        /// </summary>
        public string PerTxLimit { get; set; }

        /// <summary>
        /// 当日限额
        /// </summary>
        public string PerDayLimit { get; set; }

        /// <summary>
        /// 当日可用
        /// </summary>
        public string PerDayUsable { get; set; }

        /// <summary>
        /// 当月限额
        /// </summary>
        public string PerMonthLimit { get; set; }

        /// <summary>
        /// 当月可用
        /// </summary>
        public string PerMonthUsable { get; set; }
        /// <summary>
        /// 是否已认证营业执照
        /// </summary>
        public bool HasVerify { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        public string HandleFeeTier { get; set; }

        /// <summary>
        /// 基础手续费
        /// </summary>
        public string HandleFee { get; set; }

        /// <summary>
        /// 最小提币量
        /// </summary>
        public string MinWithdrawalAmount { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int DecimalPlace { get; set; }

        /// <summary>
        /// 提币到FiiiPay最小提币量
        /// </summary>
        public string ToFiiiPayMinWithdrawalAmount { get; set; }
        /// <summary>
        /// 是否需要Tag
        /// </summary>
        public bool NeedTag { get; set; }


        /// <summary>
        /// 认证级别
        /// </summary>
        public int VerifyLevel { get; set; }
    }
}