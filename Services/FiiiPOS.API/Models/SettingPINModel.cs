using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class SettingPINModel
    {
        /// <summary>
        /// Pin
        /// </summary>
        [Required]
        public string PIN { get; set; }
    }
}