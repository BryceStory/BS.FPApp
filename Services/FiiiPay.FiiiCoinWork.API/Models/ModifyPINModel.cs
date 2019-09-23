using System.ComponentModel.DataAnnotations;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// 修改PIN实体
    /// </summary>
    public class ModifyPINModel
    {
        /// <summary>
        /// 原PIN
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string OldPIN { get; set; }
        /// <summary>
        /// 新PIN
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string NewPIN { get; set; }
    }
}