using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO
{
    public class MerchantVerifyStatusOM
    {
        public Guid MerchantId { get; set; }
        public VerifyStatus Status { get; set; }
    }
}
