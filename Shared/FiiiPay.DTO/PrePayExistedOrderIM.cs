using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Order
{
    public class PrePayExistedOrderIM
    {
        [Required(AllowEmptyStrings = false),MaxLength(100)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单来源类型
        /// </summary>
        [Required]
        public PrePayExistedOrderIMType Type { get; set; }
    }

    public enum PrePayExistedOrderIMType
    {
        /// <summary>
        /// 推送过来的订单
        /// </summary>
        Push = 0,

        /// <summary>
        /// 扫码支付
        /// </summary>
        ScanCode = 1
    }
}
