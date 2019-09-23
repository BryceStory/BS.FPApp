using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class BillerAddress
    {
        public int Id { get; set; }
        
        public string BillerCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string Tag { get; set; }

        public string IconIndex { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid AccountId { get; set; }
    }
}
