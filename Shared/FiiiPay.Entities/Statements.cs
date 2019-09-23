using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class Statement
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public Guid AccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public BillDetailType DetailType { get; set; }
        public BillBussess BussessType { get; set; }
        public string DetailId { get; set; }
        public byte Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string MerchantName { get; set; }
    }
}
