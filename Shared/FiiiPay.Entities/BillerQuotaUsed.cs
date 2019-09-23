using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class BillerQuotaUsed
    {
        public int Id { get; set; }

        public decimal UsedFiatAmount { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
