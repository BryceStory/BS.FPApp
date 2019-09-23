using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.FiiiExTransfer
{
    /// <summary>
    /// 划转到 Ex Entity
    /// </summary>
    public class TransferToExIM
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        [Required, Plus]
        public int CryptoId { get; set; }
        /// <summary>
        /// 划转数量
        /// </summary>
        [Required, Plus]
        public decimal Amount { get; set; }

        /// <summary>
        /// PIN
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string PIN { get; set; }
    }

    /// <summary>
    /// 从 Ex 划转 Entity
    /// </summary>
    public class TransferFromExIM
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        [Required, Plus]
        public int CryptoId { get; set; }
        /// <summary>
        /// 划转数量
        /// </summary>
        [Required, Plus]
        public decimal Amount { get; set; }

        /// <summary>
        /// 校验PIN
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string PIN { get; set; }
    }
}