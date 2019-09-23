using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities.EntitySet
{
    /// <summary>
    /// 商家店面的简要消息
    /// </summary>
    public class MerchantBriefInformation
    {
        public Guid Id { get; set; }
        public string Address { get; set; }

        public string Tags { get; set; }

        public decimal Distance { get; set; }

        public string MerchantName { get; set; }
        
        public Guid FileId { get; set; }
        public Guid? ThumbnailId { get; set; }

        public Guid MerchantInformationId { get; set; }

        public byte AccountType { get; set; }

        public bool IsAllowExpense { get; set; }

        public Guid MerchantAccountId { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }
    }
    
}
