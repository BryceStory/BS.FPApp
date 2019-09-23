using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.AppVersion
{
    public class VersionIM
    {
        /// <summary>
        /// 0：ios，1：android
        /// </summary>
        [Required]
        public byte Platform { get; set; }
    }
}
