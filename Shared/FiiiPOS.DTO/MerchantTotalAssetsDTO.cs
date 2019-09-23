namespace FiiiPOS.DTO
{
    public class MerchantTotalAssetsDTO
    {
        /// <summary>
        /// 总资产
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// ≈法币（CNY,MYR,USD....）
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 是否允许提现
        /// </summary>
        public bool IsAllowWithDrawal { get; set; }
    }
}