using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Framework.MongoDB;

namespace FiiiPay.Foundation.Business.Model
{
    public class AccountActiveCountry : MongoBaseEntity
    {
        public string Country_CN { get; set; }

        public string Country_EN { get; set; }

        public int CountryId { get; set; }
        
        public Guid AccountId { get; set; }
    }
}
