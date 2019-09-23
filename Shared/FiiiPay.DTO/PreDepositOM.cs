namespace FiiiPay.DTO.Deposit
{
    public class PreDepositOM
    {
        public string Code { get; set; }
        public bool NeedTag { get; set; }

        public string Address { get; set; }
        public string Tag { get; set; }
    }
}
