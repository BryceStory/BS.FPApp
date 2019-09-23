using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class ModifyPINModel
    {
        /// <summary>
        /// 加密后新PIN
        /// </summary>
        [Required]
        public string PIN { get; set; }

    }
}