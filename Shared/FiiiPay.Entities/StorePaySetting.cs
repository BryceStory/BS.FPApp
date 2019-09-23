using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class StorePaySetting
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string FiatCurrency { get; set; }
        public decimal LimitAmount { get; set; }
    }
}
