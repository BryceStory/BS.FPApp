using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO.Order
{
    /// <summary>
    /// 扫描商家固态二维码返回信息
    /// </summary>
    public class ScanMerchantQRCodeOM
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public Guid MerchantId { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 商家头像ID
        /// </summary>
        public Guid? Avatar { get; set; }
        /// <summary>
        /// 商家LV1认证状态
        /// </summary>
        public byte L1VerifyStatus { get; set; }
        /// <summary>
        /// 商家LV2认证状态
        /// </summary>
        public byte L2VerifyStatus { get; set; }
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 溢价费率，比如，0.1，客户端需要自行转换显示为10%
        /// </summary>
        public string MarkupRate { get; set; }

        /// <summary>
        /// 加密币钱包详细列表
        /// </summary>
        public List<WalletItem> WaletInfoList { get; set; }
    }
}
