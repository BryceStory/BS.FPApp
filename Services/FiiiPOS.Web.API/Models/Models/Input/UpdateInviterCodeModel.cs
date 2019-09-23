using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Input
{
    public class UpdateInviterCodeModel
    {
        /// <summary>
        /// 邀请码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string InviterCode { get; set; }
    }
}