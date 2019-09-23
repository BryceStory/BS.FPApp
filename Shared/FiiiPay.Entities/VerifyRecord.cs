using FiiiPay.Entities.Enums;
using System;

namespace FiiiPay.Entities
{
    public class VerifyRecord
    {
        public long Id { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public VerifyRecordType Type { get; set; }
        /// <summary>
        /// 审核不成功时的内容，比如：缺少身份证正面照片
        /// </summary>
        public string Body { get; set; }

        public Guid AccountId { get; set; }

        public string Username { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public enum VerifyRecordType
    {
        UserLv1Verified = 0,
        UserLv1Reject,
        UserLv2Verified,
        UserLv2Reject,
        MerchantLv1Verified,
        MerchantLv1Reject,
        MerchantLv2Verified,
        MerchantLv2Reject,
        StoreVerified,
        StoreReject
    }
}
