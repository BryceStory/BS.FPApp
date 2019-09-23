using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class PriceInfoViewModel : PriceInfos
    {
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CryptoName { get; set; }
        public string CryptoCode { get; set; }         
    }
}