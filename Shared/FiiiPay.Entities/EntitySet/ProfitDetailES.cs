using System;
using System.Collections.Generic;

namespace FiiiPay.Entities.EntitySet
{
    public class ProfitDetailES
    {
        public int Id { get; set; }

        public int InvitationId { get; set; }

        public decimal CryptoAmount { get; set; }

        public Guid AccountId { get; set; }

        public ProfitType Type { get; set; }

        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 被邀请人id
        /// </summary>
        public Guid? InvitedAccountId { get; set; }
        
        public string PhoneCode { get; set; }

        private string _cellPhone = string.Empty;
        public string CellPhone
        {
            get => _cellPhone;
            set
            {
                _cellPhone = value;
                if(_cellPhone.Length > 4)
                {
                    _cellPhone = $"*******{_cellPhone.Substring(_cellPhone.Length - 4)}";
                }
                else
                {
                    _cellPhone = $"*******{_cellPhone}";
                }
            }
        }


    }

    
    public class TimeCompare : IComparer<ProfitDetailES>
    {
        public int Compare(ProfitDetailES x, ProfitDetailES y)
        {
            if (x.Type == ProfitType.BeInvited)
            {
                return -1;
            }
            if(y.Type == ProfitType.BeInvited)
            {
                return -1;
            }
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
}
