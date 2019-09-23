using System;

namespace FiiiPay.Entities
{
    public class UserLoginLog
    {
        public long Id { get; set; }
        public Guid UserAccountId { get; set; }
        public string IP { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string AppVersion { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool? IsSuccess { get; set; }
        public string Remark { get; set; }
    }
}
