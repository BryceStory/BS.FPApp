namespace FiiiPay.Foundation.API.Models
{
    public class ApiAddressResult
    {
        /// <summary>
        /// 0：ios，1：android
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// api地址
        /// </summary>
        public string ApiAddress { get; set; }
    }
}