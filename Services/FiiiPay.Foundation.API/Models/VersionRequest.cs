using System.ComponentModel.DataAnnotations;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.API.Models
{
    public class VersionRequest
    {
        /// <summary>
        /// 平台
        /// 0：iOS, 1：android,2:iOS 企业, 3：Android 企业
        /// </summary>
        [Required]
        public PlatfromEmum Platform { get; set; }

        /// <summary>
        /// 应用
        /// <summary>0: FiiiPay,1: FiiiPOS; 默认: 0.</summary>
        /// </summary>
        [Required]
        public AppEnum App { get; set; } = 0;
    }
}