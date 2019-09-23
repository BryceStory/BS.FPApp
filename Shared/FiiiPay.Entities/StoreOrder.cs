using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class StoreOrder
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderStatus Status { get; set; }
        public Guid MerchantInfoId { get; set; }
        public string MerchantInfoName { get; set; }
        public Guid UserAccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public decimal CryptoAmount { get; set; }
        public decimal CryptoActualAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Markup { get; set; }
        public string FiatCurrency { get; set; }
        public decimal FiatAmount { get; set; }
        public decimal FiatActualAmount { get; set; }
        public decimal FeeRate { get; set; }
        public decimal TransactionFee { get; set; }
        public DateTime? PaymentTime { get; set; }
        public string Remark { get; set; }
    }
}
