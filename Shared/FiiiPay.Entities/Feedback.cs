using System;

namespace FiiiPay.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Guid AccountId { get; set; }
        public string Context { get; set; }
        public bool HasProcessor { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
