using System;
using FiiiPay.Profile.Entities;

namespace FiiiPay.Profile.API.Models
{
    public class UpdateStatusIM
    {
        public Guid Id { get; set; }
        public VerifyStatus VerifyStatus { get; set; }
        public string Remark { get; set; }
    }
}
