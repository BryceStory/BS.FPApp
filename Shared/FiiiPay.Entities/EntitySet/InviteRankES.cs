using System;

namespace FiiiPay.Entities.EntitySet
{
    public class InviteRankES
    {
        public string PhoneCode { get; set; }
        public string CellPhone { get; set; }
        public Guid AccountId { get; set; }
        public decimal CryptoAmount { get; set; }
    }
}
