using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Order
{
    public class PrePayIM
    {
        /// <summary>
        /// 商家Id，MerchantId与MerchantCode二者必填一个，两个都有值的话使用MerchantId
        /// </summary>
        public Guid? MerchantId { get; set; }

        /// <summary>
        /// 商家15位的随机码，MerchantId与MerchantCode二者必填一个，两个都有值的话使用MerchantId
        /// </summary>
        [MaxLength(50)]
        public string MerchantCode { get; set; }
    }
}
