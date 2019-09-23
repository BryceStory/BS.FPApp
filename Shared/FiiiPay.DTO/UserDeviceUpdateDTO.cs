using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class UserDeviceUpdateIM
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Required]
        public string DeviceNumber { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
    }
}
