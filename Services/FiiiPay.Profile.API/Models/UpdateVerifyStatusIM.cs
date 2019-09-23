using System;
using FiiiPay.Profile.Entities;

namespace FiiiPay.Profile.API.Models
{
    public class UpdateVerifyStatusIM
    {
        //Guid id, VerifyStatus verifyStatus, string remark
        public Guid Id { get; set; }
        public VerifyStatus VerifyStatus { get; set; }
        public string Remark { get; set; }
    }
}
