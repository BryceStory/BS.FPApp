using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class GatewayDeductViewModel
    {
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}