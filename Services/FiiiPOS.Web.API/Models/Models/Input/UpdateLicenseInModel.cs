using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 修改认证信息
    /// </summary>
    public class UpdateLicenseInModel
    {
        /// <summary>
        /// 营业执照名称
        /// </summary>
        [Required, StringLength(50, MinimumLength = 0)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 营业执照注册号
        /// </summary>
        [Required, StringLength(50, MinimumLength = 0)]
        public string LicenseNo { get; set; }

        /// <summary>
        /// 营业执照
        /// </summary>
        [RequiredGuid]
        public string BusinessLicense { get; set; }
    }
}