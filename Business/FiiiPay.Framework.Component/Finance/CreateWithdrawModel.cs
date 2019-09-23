using System;
using Newtonsoft.Json;

namespace FiiiPay.Foundation.Business.Agent
{
    public class CreateWithdrawModel
    {
        public Guid AccountID { get; set; }
        public string CryptoName { get; set; }
        public AccountTypeEnum AccountType { get; set; }
        public string ReceivingAddress { get; set; }
        public string DestinationTag { get; set; }
        public decimal Amount { get; set; }
        public string IPAddress { get; set; }
        public decimal TransactionFee { get; set; }
        public long WithdrawalId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum AccountTypeEnum
    {
        User = 1,
        Merchant = 2
    }
}