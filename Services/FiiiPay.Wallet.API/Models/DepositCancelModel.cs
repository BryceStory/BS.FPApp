using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;

namespace FiiiPay.CryptoCurrency.API.Models
{
    /// <summary>
    /// Class FiiiPay.CryptoCurrency.API.Models.DepositRejectModel
    /// </summary>
    public class DepositCancelModel
    {
        /// <summary>
        /// Account ID 账户 ID
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        [Required]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Account Type 账户类型 （用户-1/ 商家-2）
        /// </summary>
        /// <value>
        /// The type of the account.
        /// </value>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Cryptocurrency Name, exp: BTC, ETH 加密货币简称
        /// </summary>
        /// <value>
        /// The name of the crypto.
        /// </value>
        [Required]
        public string CryptoName { get; set; }
        /// <summary>
        /// TransactionId. 交易ID
        /// </summary>
        [Required]
        public long DepositRequestId { get; set; }

        /// <summary>
        /// Timestamp. 时间戳
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }
    }
}