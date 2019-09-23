using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPos.InviteReward.ComplementData
{
    public class FiiiPayRewardMessage
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string SN { get; set; }
        public string Account { get; set; }
        public long ActualReward { get; set; }
        public bool Status { get; set; }
    }
}
