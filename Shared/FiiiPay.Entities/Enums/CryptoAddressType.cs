// ReSharper disable InconsistentNaming
namespace FiiiPay.Entities.Enums
{
    /// <summary>
    /// 地址类型
    /// </summary>
    public enum CryptoAddressType
    {
        /// <summary>
        /// FiiiPay
        /// </summary>
        FiiiPay = 1,
        /// <summary>
        /// FiiiPOS
        /// </summary>
        FiiiPOS,
        /// <summary>
        /// Outside
        /// </summary>
        Outside,
        /// <summary>
        /// 平台内的错误地址
        /// </summary>
        InsideWithError
    }
}