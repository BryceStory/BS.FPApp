using System;

namespace FiiiPay.Foundation.Entities
{
    public class Currencies
    {
        public Int16 ID { get; set; }

        public string Name { get; set; }

        public string Name_CN { get; set; }

        public string Code { get; set; }
        public bool IsFixedPrice { get; set; }
    }
}
