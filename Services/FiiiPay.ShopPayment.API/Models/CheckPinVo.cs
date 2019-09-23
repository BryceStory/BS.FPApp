using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// 校验PIN传参
    /// </summary>
    public class CheckPinVo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// PIN
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
    }
}