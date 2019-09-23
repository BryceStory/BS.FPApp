namespace FiiiPay.DTO.Withdraw
{
    public class PreWithdrawOM
    {
        public string Code { get; set; }
        public bool NeedTag { get; set; }
        /// <summary>
        /// 可用余额（=总余额-被冻结的）
        /// </summary>
        public string UseableBalance { get; set; }

        /// <summary>
        /// 最小提币数量
        /// </summary>
        public string MinAmount { get; set; }

        /// <summary>
        /// 单次限额
        /// </summary>
        public string PerTxLimit { get; set; }

        /// <summary>
        /// 当日限额
        /// </summary>
        public string PerDayLimit { get; set; }

        /// <summary>
        /// 当次可用
        /// </summary>
        public string PerTxUsable { get; set; }

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
        /// 是否已经Lv1认证
        /// </summary>
        public bool Lv1Verified { get; set; }

        /// <summary>
        /// 是否已经Lv2认证
        /// </summary>
        public bool Lv2Verified { get; set; }

        /// <summary>
        /// 如果Lv1已认证的每次限额
        /// </summary>
        public string PerTxLimitIfLv1Veried { get; set; }

        /// <summary>
        /// 如果Lv1已认证的每天限额
        /// </summary>
        public string PerDayLimitIfLv1Veried { get; set; }

        /// <summary>
        /// 如果Lv1已认证的每月限额
        /// </summary>
        public string PerMonthLimitIfLv1Veried { get; set; }

        /// <summary>
        /// 如果Lv2已认证的每次限额
        /// </summary>
        public string PerTxLimitIfLv2Veried { get; set; }

        /// <summary>
        /// 如果Lv2已认证的每天限额
        /// </summary>
        public string PerDayLimitIfLv2Veried { get; set; }

        /// <summary>
        /// 如果Lv2已认证的每月限额
        /// </summary>
        public string PerMonthLimitIfLv2Veried { get; set; }

        /// <summary>
        /// 数字币小数点位数
        /// </summary>
        public int DecimalPlace { get; set; }

        /// <summary>
        /// 提币到FiiiPay最小提币量
        /// </summary>
        public string ToFiiiPayMinWithdrawalAmount { get; set; }
    }
}
