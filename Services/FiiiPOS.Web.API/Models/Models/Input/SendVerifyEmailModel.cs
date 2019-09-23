using FiiiPay.Framework.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Input
{
    public class SendVerifyEmailModel
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        [StringLength(50)]
        public string EmailAddress { get; set; }
    }
}