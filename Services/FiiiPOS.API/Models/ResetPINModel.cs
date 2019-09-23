using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.API.Models
{
    public class ResetPINModel
    {
        /// <summary>
        /// Pin
        /// </summary>
        [Required]
        public string PIN { get; set; }
    }
}