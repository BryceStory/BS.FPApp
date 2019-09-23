using FiiiPay.Framework;
using FiiiPay.Framework.Component;

namespace FiiiPOS.Business
{
    /// <summary>
    /// Class FiiiPay.Framework.MerchantAccessToken
    /// </summary>
    public class MerchantAccessToken : AccessToken
    {
        /// <summary>
        /// Gets or sets the possn.
        /// </summary>
        /// <value>
        /// The possn.
        /// </value>
        public string POSSN { get; set; }

        public override SystemPlatform Platform { get; set; } = SystemPlatform.FiiiPOS;
    }
}