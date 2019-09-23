using FiiiPay.Framework.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.CryptoCurrency.API.Models
{
    /// <summary>
    /// 挖矿确认数据列表
    /// </summary>
    public class MiningConfirmedModel
    {
        /// <summary>
        /// data list to confirm
        /// </summary>
        [Required]
        public List<MiningConfirmed> MiningConfirmedList { get; set; }
    }
    /// <summary>
    /// data to confirm
    /// </summary>
    public class MiningConfirmed
    {
        /// <summary>
        /// 账户类型 1 FiiiPay, 2 FiiiPos
        /// </summary>
        [Required]
        public byte AccountType { get; set; }
        /// <summary>
        /// merchant accountId
        /// </summary>
        [Required]
        public Guid AccountId { get; set; }
        /// <summary>
        /// unfrozen amount
        /// </summary>
        [Required, Plus]
        public decimal Amount { get; set; }
    }
}