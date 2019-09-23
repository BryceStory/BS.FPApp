using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class ScanQRLoginInModel
    {
        /// <summary>
        /// 扫描获得的二维码
        /// </summary>
        [Required]
        public string QRCode { get; set; }
    }
}