using FiiiPay.Framework.Component.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.ShopPayment.API.Models
{
    public class LoginInfo
    {
        public Guid AccountId { get; set; }
        public LoginStatus Status { get; set; }
    }
}