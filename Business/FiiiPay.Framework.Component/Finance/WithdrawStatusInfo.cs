namespace FiiiPay.Foundation.Business.Agent
{
    public class WithdrawStatusInfo
    {
        public byte RequestStatus { get; set; }
        public byte TransactionStatus { get; set; }
        public string Remarks { get; set; }
        public int TotalConfirmation { get; set; }
        public int MinRequiredConfirmation { get; set; }
        public string TransactionID { get; set; }
    }

    public class DepositStatusInfo
    {
        public byte Status { get; set; }
        public string TransactionID { get; set; }
        public int TotalConfirmation { get; set; }
        public int MinRequiredConfirmation { get; set; }
    }

}