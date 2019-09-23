using System;

namespace FiiiPay.Entities
{
    public class ReadRecord
    {
        public long Id { get; set; }
        public ReadRecordType Type { get; set; }
        public Guid AccountId { get; set; }
        public string TargetId { get; set; }
    }

    public enum ReadRecordType
    {
        Article = 0, Verify
    }
}
