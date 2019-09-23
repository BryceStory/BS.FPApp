using System.ComponentModel.DataAnnotations;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// 修改密码实体
    /// </summary>
    public class ModifyPasswordModel
    {
        /// <summary>
        /// 原密码
        /// </summary>
        [Required, StringLength(20, MinimumLength = 8)]
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required, StringLength(20, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}