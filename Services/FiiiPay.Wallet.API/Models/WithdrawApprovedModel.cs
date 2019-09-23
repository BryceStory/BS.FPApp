using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;

namespace FiiiPay.CryptoCurrency.API.Models
{
    /// <summary>
    /// 提现审核通过
    /// </summary>
    public class WithdrawApprovedModel
    {
        /// <summary>
        /// Account Type 账户类型 （用户-1/ 商家-2）
        /// </summary>
        /// <value>
        /// The type of the account.
        /// </value>
        [Required]
        public AccountType AccountType { get; set; }
        /// <summary>
        /// Account ID 账户 ID
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        [Required]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Gets or sets the withdraw request identifier.
        /// </summary>
        /// <value>
        /// The withdraw request identifier.
        /// </value>
        [Required]
        public long WithdrawRequestId { get; set; }
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
        public string TransactionId { get; set; }
    }
}