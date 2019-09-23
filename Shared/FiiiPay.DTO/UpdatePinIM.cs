using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class UpdatePinIM
    {
        /// <summary>
        /// 加密过的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(256)]
        public string OldPin { get; set; }

        /// <summary>
        /// 加密过的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(256)]
        public string NewPin { get; set; }
    }
}
