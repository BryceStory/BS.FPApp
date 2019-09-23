namespace FiiiPOS.DTO
{
    /// <summary>
    /// 充币地址信息
    /// </summary>
    public class DepositAddressInfo
    {
        /// <summary>
        /// 是否需要tag
        /// </summary>
        public bool NeedTag { get; set; }
        /// <summary>
        /// 充币地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// tag
        /// </summary>
        public string Tag { get; set; }
    }
}