using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Models
{
    [SugarTable("BalanceStatements")]
    public class BalanceStatement
    {
        public long Id { get; set; }

        public Guid AccountId { get; set; }

        public string Action { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public DateTime Timestamp { get; set; }

        public string Remark { get; set; }
    }
}