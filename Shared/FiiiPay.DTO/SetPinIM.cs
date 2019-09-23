using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class SetPinIM
    {
        /// <summary>
        /// 所有Pin都需要加密传到服务端
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
    }
}
