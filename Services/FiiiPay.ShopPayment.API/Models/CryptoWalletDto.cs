namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class CryptoWalletDto
    /// </summary>
    public class CryptoWalletDto
    {
        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the frozen balance.
        /// </summary>
        /// <value>
        /// The frozen balance.
        /// </value>
        public decimal FrozenBalance { get; set; }
    }
}