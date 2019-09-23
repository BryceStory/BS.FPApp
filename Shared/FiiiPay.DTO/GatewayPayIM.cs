using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.GatewayOrder
{
    public class GatewayPayIM
    {
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }
        [Required,RequiredGuid]
        public Guid OrderId { get; set; }
    }
}
