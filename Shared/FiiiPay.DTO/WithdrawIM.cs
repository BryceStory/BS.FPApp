using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Withdraw
{
    public class WithdrawIM
    {
        [Required, Plus]
        public int CoinId { get; set; }
        [Required,MaxLength(256)]
        public string Address { get; set; }
        
        public string Tag { get; set; }
        [Required,Plus]
        public decimal Amount { get; set; }
    }
}
