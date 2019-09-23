using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 商家头像
    /// </summary>
    public class HeadImageInModel
    {
        /// <summary>
        /// PhotoId
        /// </summary>
        [RequiredGuid]
        public string PhotoId { get; set; }
    }
}