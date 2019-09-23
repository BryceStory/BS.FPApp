using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class UserDevice
    {
        public long Id { get; set; }
        public Guid UserAccountId { get; set; }
        public string DeviceNumber { get; set; }
        public string IP { get; set; }
        public DateTime LastActiveTime { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
