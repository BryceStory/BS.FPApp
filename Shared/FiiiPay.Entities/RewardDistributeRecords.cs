using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class RewardDistributeRecords
    {
        public long Id { get; set; }
        public Guid UserAccountId { get; set; }
        public Guid MerchantAccountId { get; set; }
        public string SN { get; set; }
        public long OriginalReward { get; set; }
        public decimal ActualReward { get; set; }
        public decimal Percentage { get; set; }
        public DateTime Timestamp { get; set; }
        public long ProfitId { get; set; }
    }
}
