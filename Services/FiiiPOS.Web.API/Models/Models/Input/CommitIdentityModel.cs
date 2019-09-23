using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.Web.API.Models.Input
{
    public class CommitIdentityModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public IdentityDocType IdentityDocType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string IdentityDocNo { get; set; }

        /// <summary>
        /// 证件正面照
        /// </summary>
        [RequiredGuid]
        public Guid FrontIdentityImage { get; set; }

        /// <summary>
        /// 证件反面照
        /// </summary>
        [RequiredGuid]
        public Guid BackIdentityImage { get; set; }

        /// <summary>
        /// 手持证件照
        /// </summary>
        [RequiredGuid]
        public Guid HandHoldWithCard { get; set; }
    }
}