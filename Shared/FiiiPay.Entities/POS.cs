using System;

namespace FiiiPay.Entities
{
    public class POS
    {
        public long Id { get; set; }
        public string Sn { get; set; }
        public bool IsWhiteLabel { get; set; }
        public string WhiteLabel { get; set; }
        public string FirstCrypto { get; set; }
        public bool Status { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsMiningEnabled { get; set; }
    }

    public enum POSStatus
    {
        Inactived = 0,
        Actived = 1
    }
}
