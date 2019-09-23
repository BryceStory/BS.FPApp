using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class RefundVo
    /// </summary>
    public class RefundVo
    {
        /// <summary>
        /// Gets or sets the order no.
        /// </summary>
        /// <value>
        /// The order no.
        /// </value>
        [Required]
        public string OrderId { get; set; }

        /// <summary>
        /// 退款加密币数量
        /// </summary>
        /// <value>
        /// The crypto amount.
        /// </value>
        [Required]
        public decimal CryptoAmount { get; set; }
    }
}