namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// Basic AccessToken
    /// </summary>
    public class AccessToken : AccessTokenWithTId<string>
    {
        public virtual SystemPlatform Platform { get; set; } = SystemPlatform.FiiiPay;
    }

    /// <summary>
    /// Type of AccessToken
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AccessTokenWithTId<T>
    {
        /// <summary>
        /// T Identity
        /// </summary>
        public T Identity { get; set; }
    }
}