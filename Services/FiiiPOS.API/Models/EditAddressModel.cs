using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    public class EditAddressModel
    {
        /// <summary>
        /// 地址ID
        /// </summary>
        [Required, MathRange(0, long.MaxValue)]
        public long Id { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Required]
        public string Address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        public string Remark { get; set; }
    }
}