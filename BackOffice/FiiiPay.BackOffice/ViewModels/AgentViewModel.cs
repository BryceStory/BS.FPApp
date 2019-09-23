using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class AgentViewModel : Agent
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string SaleName { get; set; }
        public string SaleCode { get; set; }
        public string AgentTime { get; set; }
    }
}