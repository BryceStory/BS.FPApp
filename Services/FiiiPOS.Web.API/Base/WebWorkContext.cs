using System;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class WebWorkContext
    {
        /// <summary>
        /// 商家Id
        /// </summary>
        public Guid MerchantId { get; set; }
        /// <summary>
        /// 商家账户
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 所在国家Id
        /// </summary>
        public int CountryId { get; set; }

    }
}