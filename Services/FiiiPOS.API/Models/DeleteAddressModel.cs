using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    public class DeleteAddressModel
    {
        /// <summary>
        /// AddressID
        /// </summary>
        [Required, MathRange(0, long.MaxValue)]
        public long Id { get; set; }
    }
}