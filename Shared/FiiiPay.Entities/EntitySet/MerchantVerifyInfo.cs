using System;

namespace FiiiPay.Entities.EntitySet
{
    /// <summary>
    /// POS机商家认证审核
    /// </summary>
    public class MerchantVerifyInfo
    {
        public Guid? MerchantId { set; get; }
        public string Cellphone { get; set; }
        public int Country { set; get; }        
        public int VerifyStatus { get; set; }
        public DateTime SubmissionDate { get; set; }     
        public string MerchantName { get; set; }      
        public long? POSId { get; set; } 
        public string Remark { get; set; }
    }
}
