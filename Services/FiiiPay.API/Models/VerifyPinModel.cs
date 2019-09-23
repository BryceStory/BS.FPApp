using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class VerifyPinModel
    {
        /// <summary>
        /// 加密的Pin
        /// </summary>
        [Required]
        public string PIN { get; set; }
    }
}