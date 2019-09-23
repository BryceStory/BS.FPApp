namespace FiiiPay.Foundation.API.Models
{
    public class AccessControlListModel
    {
        public AccessModel Access { get; set; }
        public string Username { get; set; }
        public string ClientId { get; set; }
        public string Ipaddr { get; set; }
        /// <summary>
        /// $platform/business/identity
        /// </summary>
        public string Topic { get; set; }
    }

    public enum AccessModel
    {
        /// <summary>
        /// 订阅
        /// </summary>
        Subscribe = 1,
        /// <summary>
        /// 发布
        /// </summary>
        Publish = 2
    }
}