using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class POSMerchantBindRecord
    {
        public long Id { get; set; }
        public long POSId { get; set; }
        public string SN { get; set; }
        public Guid MerchantId { get; set; }
        public string MerchantUsername { get; set; }
        public DateTime BindTime { get; set; }
        public DateTime? UnbindTime { get; set; }
        /// <summary>
        /// 绑定状态 0未绑定|已解绑 1已绑定
        /// </summary>
        public byte BindStatus { get; set; }
    }
}
