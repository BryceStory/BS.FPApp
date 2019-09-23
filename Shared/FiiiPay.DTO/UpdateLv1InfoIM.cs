using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Profile
{
    public class UpdateLv1InfoIM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public IdentityDocType? IdentityDocType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 1)]
        public string IdentityDocNo { get; set; }

        /// <summary>
        /// 证件正面照
        /// </summary>
        [Required, RequiredGuid]
        public Guid FrontIdentityImage { get; set; }

        /// <summary>
        /// 证件反面照
        /// </summary>
        [Required, RequiredGuid]
        public Guid BackIdentityImage { get; set; }

        /// <summary>
        /// 手持证件照
        /// </summary>
        [Required, RequiredGuid]
        public Guid HandHoldWithCard { get; set; }
    }
}
