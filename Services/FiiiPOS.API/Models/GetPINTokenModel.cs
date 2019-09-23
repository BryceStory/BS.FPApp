using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class GetPINTokenModel
    {
        /// <summary>
        /// 加密的Pin
        /// </summary>
        [Required]
        public string PIN { get; set; }

    }
}