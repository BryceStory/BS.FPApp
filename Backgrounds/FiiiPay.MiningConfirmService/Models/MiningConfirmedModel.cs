using System;

namespace FiiiPay.MiningConfirmService.Models
{
    public class MiningConfirmedModel
    {
        public byte AccountType { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
