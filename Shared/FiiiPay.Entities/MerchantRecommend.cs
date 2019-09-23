using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
   public class MerchantRecommend
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 商家信息id
        /// </summary>
        public Guid MerchantInformationId { get; set; }

        /// <summary>
        /// 商家推荐内容
        /// </summary>
        public string RecommendContent { get; set; }

        /// <summary>
        /// 商家推荐图片
        /// </summary>
        public Guid RecommendPicture { get; set; }

        /// <summary>
        /// 排序图片
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 缩略图id
        /// </summary>
        public Guid ThumbnailId { get; set; }
    }
}
