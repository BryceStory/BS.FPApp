using System;

namespace FiiiPay.Entities.EntitySet
{
    public class Address
    {
        public Guid Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
    }
}
