using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class UpdateNicknameModel
    {
        /// <summary>
        /// 昵称
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        [Required]
        public string Nickname { get; set; }
    }
}