namespace FiiiPay.DTO.FiiiExTransfer
{
    public class TransferFiiiExConditionOM
    {
        /// <summary>
        /// 可用余额
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// 最低划转数量
        /// </summary>
        public string MinQuantity { get; set; }
        /// <summary>
        /// FiiiEx可用余额
        /// </summary>
        public string FiiiExBalance { get; set; }
        /// <summary>
        /// FiiiEx最低划转数量
        /// </summary>
        public string FiiiExMinQuantity { get; set; }
    }
}