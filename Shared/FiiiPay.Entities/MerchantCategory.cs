using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
   public class MerchantCategory
    {
        /// <summary>
        /// 商家信息ID
        /// </summary>
        public Guid MerchantInformationId { get; set; }

        /// <summary>
        /// BO类别ID
        /// </summary>
        public int Category { get; set; }

      
    }
}
