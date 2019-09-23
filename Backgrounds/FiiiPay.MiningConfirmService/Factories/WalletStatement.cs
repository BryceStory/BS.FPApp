using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.MiningConfirmService.Factories
{
    public class WalletStatement
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenAmount { get; set; }
        public decimal FrozenBalance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}
