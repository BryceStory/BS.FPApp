using FiiiPay.Entities.Enums;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public class Withdraw
    {
        public TransactionStatus Status { get; set; }
        public decimal Amount { get; set; }
        public long Id { get; set; }
        public string TransactionId { get; set; }
    }
}