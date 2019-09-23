using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class VerifyUpdatePasswordModel
    {
        /// <summary>
        /// 短信验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Pin { get; set; }
    }
}