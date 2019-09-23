using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPos.InviteReward
{
    public class FiiiPayRewardMessage
    {
        public string Address { get; set; }
        public string SN { get; set; }
        public string Account { get; set; }
        public long Reward { get; set; }
        public long CurrentDate { get; set; }
    }
}
