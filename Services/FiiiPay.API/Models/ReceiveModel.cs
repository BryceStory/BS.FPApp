using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// Class FiiiPay.API.Models.ReceiveModel
    /// </summary>
    public class ReceiveModel
    {
        /// <summary>
        /// 口令
        /// </summary>
        /// <value>
        /// The pass code.
        /// </value>
        [Required]
        public string PassCode { get; set; }
    }
}