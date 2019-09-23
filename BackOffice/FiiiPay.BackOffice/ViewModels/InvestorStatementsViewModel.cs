using FiiiPay.Framework.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.ViewModels
{
    public class InvestorStatementsViewModel
    {
        public Guid Id { get; set; }
        public int InvestorId { get; set; }
        public string Cellphone { get; set; }
        public string Username { get; set; }
        public string InvestorName { get; set; }
        public InvestorTransactionType Action { get; set; }
        public Decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}