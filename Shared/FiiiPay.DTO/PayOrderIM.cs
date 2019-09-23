using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Order
{
    public class PayOrderIM
    {
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 加密多的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
    }
}
