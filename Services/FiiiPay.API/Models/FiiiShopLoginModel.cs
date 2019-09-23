using FiiiPay.Framework.Component.Enums;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// Logined
    /// </summary>
    public class FiiiShopLoginModel
    {
        /// <summary>
        /// 用户唯一标志
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// 登录状态
        /// </summary>
        public LoginStatus Status { get; set; }
    }
}