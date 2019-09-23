using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.GatewayOrder
{
    public class GatewayPrePayIM
    {
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
