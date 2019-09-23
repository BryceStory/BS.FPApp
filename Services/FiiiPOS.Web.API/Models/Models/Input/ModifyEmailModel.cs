using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Input
{
    public class ModifyEmailModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }

        /// <summary>
        /// 验证PIN时产生的Token
        /// </summary>
        [Required]
        public string Token { get; set; }
    }
}