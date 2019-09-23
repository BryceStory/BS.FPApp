using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities.EntitySet
{
    public class UserAccountStatus
    {       
        public Guid? UserAccountId { get; set; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string Cellphone { get; set; }      
        public int? Country { get; set; }      
        public string Remark { get; set; }
        public byte? Status { get; set; }
        /// <summary>
        /// 身份信息认证
        /// </summary>
        public VerifyStatus? L1VerifyStatus { get; set; }
        /// <summary>
        /// 住址认证
        /// </summary>
        public VerifyStatus? L2VerifyStatus { get; set; }
        public DateTime? LastLoginTimeStamp { get; set; }
        public DateTime? RegistrationDate { get; set; }
        /// <summary>
        /// 提币功能
        /// </summary>
        public bool? IsAllowWithdrawal { get; set; }
        /// <summary>
        /// 消费功能
        /// </summary>
        public bool? IsAllowExpense { get; set; }

    }
}
