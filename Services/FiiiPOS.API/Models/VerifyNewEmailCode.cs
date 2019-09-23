using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.API.Models
{
    public class VerifyNewEmailCode
    {

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string Code { get; set; }
    }
}