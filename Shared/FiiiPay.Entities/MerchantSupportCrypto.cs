using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class MerchantSupportCrypto
    {
        public Guid MerchantInfoId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
    }
}
