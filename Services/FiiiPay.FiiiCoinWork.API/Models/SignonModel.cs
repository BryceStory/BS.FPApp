using System.ComponentModel.DataAnnotations;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// 登录Model
    /// </summary>
    public class SignonModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}