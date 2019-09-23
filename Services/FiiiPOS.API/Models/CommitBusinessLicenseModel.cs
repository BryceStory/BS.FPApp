using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    public class CommitBusinessLicenseModel
    {
        /// <summary>
        /// 营业执照名称
        /// </summary>
        [Required, StringLength(50, MinimumLength = 0)]
        public string Name { get; set; }
        /// <summary>
        /// 营业执照注册号
        /// </summary>
        [Required, StringLength(50, MinimumLength = 0)]
        public string Number { get; set; }
        /// <summary>
        /// 上传营业执照获得的GUID
        /// </summary>
        [RequiredGuid]
        public Guid Image { get; set; }
    }
}