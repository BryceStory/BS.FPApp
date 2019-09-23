using System;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class MerchantVerifyStatusOM
    {
        public Guid MerchantId { get; set; }
        public VerifyStatus Status { get; set; }
    }
}
