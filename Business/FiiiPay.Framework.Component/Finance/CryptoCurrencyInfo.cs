namespace FiiiPay.Foundation.Business.Agent
{
    public class CryptoCurrencyInfo
    {
        public int CryptoID { get; set; }
        public string CryptoName { get; set; }
        public string CryptoDesc { get; set; }
        public byte DecimalPlaces { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal DailyWithdrawalLimit { get; set; }
        public decimal MinWithdrawalAmount { get; set; }
    }
}