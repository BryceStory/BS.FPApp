using FiiiPay.Framework.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.CryptoCurrency.API.Models
{
    /// <summary>
    /// Received notification that the mine has been mined
    /// </summary>
    public class MiningReceivedModel
    {
        /// <summary>
        /// 请求ID, 唯一标志
        /// </summary>
        [Required]
        public long RequestId { get; set; }
        /// <summary>
        /// TransactionId
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string TransactionId { get; set; }
        /// <summary>
        /// 账户类型 1 FiiiPay, 2 FiiiPos
        /// </summary>
        [Required]
        public byte AccountType { get; set; }
        /// <summary>
        /// merchant accountId
        /// </summary>
        [Required]
        public Guid AccountId { get; set; }
        /// <summary>
        /// frozen amount
        /// </summary>
        [Required, Plus]
        public decimal Amount { get; set; }
    }
}