using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    /// <summary>
    /// 地图搜索入参
    /// </summary>
    public class GetMerchantInfoListByMapIM
    {
        /// <summary>
        /// 左上方
        /// </summary>
        public Location LeftTop { get; set; }

        /// <summary>
        /// 右下方
        /// </summary>
        public Location RightDown { get; set; }
    }

    /// <summary>
    /// 地图搜索 当前位置附近所有商家
    /// </summary>
    public class GetMerchantInfoListByDistanceIM
    {
        /// <summary>
        /// 当前位置
        /// </summary>
        [Required]
        public Location CurrentPlace { get; set; }

        /// <summary>
        /// 距离范围
        /// </summary>
        public decimal Distance { get; set; } = 50000;
    }

}
