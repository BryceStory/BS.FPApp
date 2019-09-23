using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInModel
    {
        /// <summary>
        /// 二维码
        /// </summary>
        [Required]
        public string QRCode { get; set; }
    }

    /// <summary>
    /// 测试专用
    /// </summary>
    public class ScanQRLoginInModel
    {

        /// <summary>
        /// 商家Id
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestLoginInModel
    {
        /// <summary>
        /// 测试用
        /// </summary>
        public string MerchantId { get; set; }

    }

}