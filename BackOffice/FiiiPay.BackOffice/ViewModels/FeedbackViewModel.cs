using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Guid AccountId { get; set; }
        public string Context { get; set; }
        public bool HasProcessor { get; set; }
        public DateTime Timestamp { get; set; }
        public string AccountName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}