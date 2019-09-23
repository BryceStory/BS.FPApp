using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class WalletsViewModel
    {
        public string CurrencyName { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
    }
}