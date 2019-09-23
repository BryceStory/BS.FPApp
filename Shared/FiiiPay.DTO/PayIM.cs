using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Order
{
    public class PayIM
    {
        /// <summary>
        /// 法币的金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种Id
        /// </summary>
        [Required]
        public int CoinId { get; set; }

        /// <summary>
        /// 商家Id，MerchantId与MerchantCode二者必填一个，两个都有值的话使用MerchantId
        /// </summary>
        public Guid? MerchantId { get; set; }

        /// <summary>
        /// 商家15位的随机码，MerchantId与MerchantCode二者必填一个，两个都有值的话使用MerchantId
        /// </summary>
        [MaxLength(50)]
        public string MerchantCode { get; set; }

        /// <summary>
        /// 加密过后的Pin
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Pin { get; set; }

        /// <summary>
        /// 支付类型(蓝牙，NFC，扫码)
        /// </summary>
        [Required]
        public PaymentType PaymentType { get; set; }
    }
}
