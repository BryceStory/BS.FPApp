namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class LoginDto
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 安全邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 所在国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 所在省、州名
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 所在城市名
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postcode { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Cellphone { get; set; }
    }
}