using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class BillerOrderViewModel
    {
        public Guid Id { get; set; }

        public string OrderNo { get; set; }

        public string AccountNo { get; set; }

        public int CountryId { get; set; }

        public string BillerCode { get; set; }

        public string ReferenceNumber { get; set; }        

        public decimal FiatAmount { get; set; }

        public string FiatCurrency { get; set; }

        public int CryptoId { get; set; }

        public decimal CryptoAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime Timestamp { get; set; }

        public BillerOrderStatus Status { get; set; }

        public string Tag { get; set; }
    }
}