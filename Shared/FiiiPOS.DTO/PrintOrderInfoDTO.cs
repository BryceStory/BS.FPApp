using System;

namespace FiiiPOS.DTO
{
    public class PrintOrderInfoDTO : OrderDetailDTO
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 头像图片ID
        /// </summary>
        public string AvatarId { get; set; }

        /// <summary>
        /// 币种图片ID
        /// </summary>
        public Guid? CryptoImage { get; set; }
    }
}