using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class BillerOrder
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public decimal FiatAmount { get; set; }

        public decimal CryptoAmount { get; set; }

        public string BillerCode { get; set; }

        public string ReferenceNumber { get; set; }

        public int CryptoId { get; set; }

        public string CryptoCode { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime PayTime { get; set; }

        public decimal Discount { get; set; }

        public BillerOrderStatus Status { get; set; }

        public string Tag { get; set; }

        public string FiatCurrency { get; set; }

        public string OrderNo { get; set; }

        public int CountryId { get; set; }

        public string Remark { get; set; }

        public DateTime FinishTime { get; set; }
    }

    public enum BillerOrderStatus : byte
    {
        //进行中
        Pending,
        //失败
        Fail,
        //完成
        Complete
    }
}
