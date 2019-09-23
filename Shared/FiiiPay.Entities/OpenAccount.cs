using System;

namespace FiiiPay.Entities
{
    public class OpenAccount
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public int PlatformId { get; set; }
        /// <summary>
        /// 0FiiiPay 1FiiiPos
        /// </summary>
        public FiiiType FiiiType { get; set; }
        public Guid AccountId { get; set; }
        public Guid OpenId { get; set; }
        public string SecretKey { get; set; }
    }

    public enum FiiiType
    {
        FiiiPay,
        FiiiPOS
    }
}
