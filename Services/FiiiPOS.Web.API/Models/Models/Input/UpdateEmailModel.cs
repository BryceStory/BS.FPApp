using FiiiPay.Framework.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Input
{
    public class UpdateEmailModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        public string Email { get; set; }
    }
}