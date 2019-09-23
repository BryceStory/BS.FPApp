using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
   public class MerchantOwnersFigure
    {
        /// <summary>
        /// 商家信息ID
        /// </summary>
        public Guid MerchantInformationId { get; set; }

        /// <summary>
        /// 门店主图ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 图片排序标识
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 缩略图id
        /// </summary>
        public Guid ThumbnailId { get; set; }
    }
}
