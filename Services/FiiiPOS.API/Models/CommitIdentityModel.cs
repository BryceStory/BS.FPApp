﻿using FiiiPay.Framework.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.API.Models
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
        public Guid FrontIdentityImage { get; set; }

        /// <summary>
        /// 证件反面照
        /// </summary>
        public Guid BackIdentityImage { get; set; }

        /// <summary>
        /// 手持证件照
        /// </summary>
        public Guid HandHoldWithCard { get; set; }
    }
}