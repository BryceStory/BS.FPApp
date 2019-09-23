using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
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