using FiiiPay.Foundation.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Models
{
    [SugarTable("PriceInfo")]
    public class PriceInfos
    {
        public int ID { get; set; }

        public int CryptoID { get; set; }

        public int CurrencyID { get; set; }

        public decimal Price { get; set; }
        [SugarColumn(IsIgnore = true)]
        public decimal MarketPrice { get; set; }
        public decimal Markup { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }
}