using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.MessageWorkerService
{
    public class StoreOrderMessage
    {
        public Guid Id { get; set; }
        public Guid MerchantInfoId { get; set; }
        public Guid UserAccountId { get; set; }
        public string CryptoCode { get; set; }
    }

    public class FiiiPayMerchantProfileVerified
    {
        public Guid AccountId { get; set; }
        public long VerifyRecordId { get; set; }
        public Entities.Enums.VerifyStatus VerifyResult { get; set; }
    }
}
