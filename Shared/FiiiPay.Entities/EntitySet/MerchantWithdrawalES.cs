namespace FiiiPay.Entities.EntitySet
{
    public class MerchantWithdrawalES : MerchantWithdrawal
    {
        public decimal WithdrawalFee { get; set; }
        public int CryptoId { get; set; }
    }
}