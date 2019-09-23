using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class StoreInfoViewModel
    {
        public string MerchantName { get; set; }
        public List<StoreTypes> StoreType { get; set; }
        public string Week { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Tags { get; set; }
        public string Phone { get; set; }
        public string Introduce { get; set; }
        public string Address { get; set; }
        public List<Guid> FigureImage { get; set; }
        public List<Guid> RecommendImage { get; set; }
    }
}