using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class OrderRefundModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        public string OrderNo { get; set; }
        /// <summary>
        /// 验证PIN后返回的Token
        /// </summary>
        [Required]
        public string PINToken { get; set; }
    }
}