using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantDetail
    {
        public string MerchantName { get; set; }

        /// <summary>
        /// string可序列化list类型
        /// </summary>
        public string Tags { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Address { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public string Introduce { get; set; }
    }
}
