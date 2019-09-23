using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.FiiiExTransfer
{
    /// <summary>
    /// 查询FiiiEx余额入参
    /// </summary>
    public class FiiiExBalanceIM
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        [Required, Plus]
        public int CyptoId { get; set; }
    }
}