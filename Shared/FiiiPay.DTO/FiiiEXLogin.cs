using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.FiiiEXLogin
{
    /// <summary>
    /// FiiiPay扫码登录FiiiEX
    /// </summary>
    public class FiiiEXLoginIM
    {
        /// <summary>
        /// 二维码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
