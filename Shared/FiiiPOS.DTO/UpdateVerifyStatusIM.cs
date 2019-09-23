using System;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class UpdateVerifyStatusIM
    {
        //Guid id, VerifyStatus verifyStatus, string remark
        public Guid Id { get; set; }
        public VerifyStatus VerifyStatus { get; set; }
        public string Remark { get; set; }
    }
}
