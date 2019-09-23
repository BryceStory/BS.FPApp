using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// Class FiiiPay.API.Models.PushRedPocketModel
    /// </summary>
    public class PushRedPocketModel
    {
        /// <summary>
        /// 加密币ID
        /// </summary>
        /// <value>
        /// The crypto identifier.
        /// </value>
        [Required]
        public int CryptoId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 个数
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Required]
        public int Count { get; set; }

        /// <summary>
        /// PIN
        /// </summary>
        /// <value>
        /// The pin.
        /// </value>
        public string PIN { get; set; }

        /// <summary>
        /// 祝福消息
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [MaxLength(50)]
        public string Message { get; set; }
    }
}