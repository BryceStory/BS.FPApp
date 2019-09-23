using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class GetUpdateCellphoneCodeIM
    {
        /// <summary>
        /// 加密过的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
    }
}
