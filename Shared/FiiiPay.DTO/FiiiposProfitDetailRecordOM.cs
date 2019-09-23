using System.Collections.Generic;

namespace FiiiPay.DTO.Invite
{
    public class FiiiposProfitDetailRecordOM
    {
        public string MerchantName { get; set; }

        public string TotalAmount { get; set; }

        public List<BonusDetail> Data { get; set; }

        public class BonusDetail
        {
            /// <summary>
            /// 收益日期
            /// </summary>
            public string Timestamp { get; set; }
            /// <summary>
            /// 具体金额
            /// </summary>
            public string CryptoAmount { get; set; }
        }
        
    }
}
