using System.ComponentModel.DataAnnotations;
using FiiiPOS.Business.Properties;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 校验商家信息model
    /// </summary>
    public class VerifyAccountModel
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        [Required, StringLength(36)]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商家帐号
        /// </summary>
        [Required, StringLength(20, MinimumLength = 3), RegularExpression("^[A-Za-z]+[A-Za-z0-9]*$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "格式不正确")]
        public string MerchantAccount { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
    }
}