using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    public class AddAddressModel
    {
        /// <summary>
        /// Cryptocurrency Id
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CryptoId { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Required, StringLength(300)]
        public string Address { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Required, StringLength(50)]
        public string Remark { get; set; }
    }
}