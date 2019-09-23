using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class PaymentVo
    /// </summary>
    public class PaymentVo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 商城订单号
        /// </summary>
        /// <value>
        /// The merchant order identifier.
        /// </value>
        [Required]
        public string OrderId { get; set; }

        /// <summary>
        /// PIN
        /// </summary>
        /// <value>
        /// The pin.
        /// </value>
        [Required]
        public string PIN { get; set; }

        /// <summary>
        /// 支付加密币数量
        /// </summary>
        /// <value>
        /// The crypto amount.
        /// </value>
        [Required]
        public decimal CryptoAmount { get; set; }
    }
}