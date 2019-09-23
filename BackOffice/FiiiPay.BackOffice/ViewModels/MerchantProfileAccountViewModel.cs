using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class MerchantProfileAccountViewModel
    {
        public Guid MerchantId { get; set; }
        public string Cellphone { get; set; }
        public string MerchantName { get; set; }
        public string SN { get; set; }
        public string Username { get; set; }
    }
}