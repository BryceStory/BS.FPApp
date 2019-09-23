using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Invite
{
    public class FiiiposBonusRecordIM
    {
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string MerchantName { get; set; }
        [Required,Plus(true)]
        public decimal TotalCryptoAmount { get; set; }
        /// <summary>
        /// 作为下次获取商家分红详情的入参
        /// </summary>
        [Required, RequiredGuid]
        public Guid MerchantId { get; set; }
    }
}
