using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class UserDeviceItemOM
    {
        /// <summary>
        /// 设备id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        public string DeviceNumber { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 最近活跃时间
        /// </summary>
        public string LastActiveTime { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
    }
}
