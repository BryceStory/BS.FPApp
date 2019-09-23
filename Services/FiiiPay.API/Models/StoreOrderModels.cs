using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class PreStorePayModel
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        [RequiredGuid]
        public Guid MerchantInfoId { get; set; }
    }

    /// <summary>
    /// 门店订单支付信息
    /// </summary>
    public class StoreOrderPayModel
    {
        /// <summary>
        /// 法币额度
        /// </summary>
        [Range(0, 9999999999.99)]
        public decimal FiatAmount { get; set; }
        /// <summary>
        /// 支付币种ID
        /// </summary>
        [MathRange(1,int.MaxValue)]
        public int CoinId { get; set; }
        /// <summary>
        /// 门店ID
        /// </summary>
        [RequiredGuid]
        public Guid MerchantInfoId { get; set; }
        /// <summary>
        /// Pin
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Pin { get; set; }
    }
}