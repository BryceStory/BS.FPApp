using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Profile
{
    public class PreVerifyLv1OM
    {
        public string Fullname { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public IdentityDocType? IdentityDocType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdentityDocNo { get; set; }

        /// <summary>
        /// 证件正面照
        /// </summary>
        public Guid? FrontIdentityImage { get; set; }

        /// <summary>
        /// 证件反面照
        /// </summary>
        public Guid? BackIdentityImage { get; set; }

        /// <summary>
        /// 手持证件照
        /// </summary>
        public Guid? HandHoldWithCard { get; set; }
    }
}
