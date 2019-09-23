using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class GetUpdatePinCodeIM
    {
        /// <summary>
        /// 加密过的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
    }
}
