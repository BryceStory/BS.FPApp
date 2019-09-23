using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Article
{
    public class ArticleListIM
    {
        [Required,Plus]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 从0开始
        /// </summary>
        [Required,Plus(true)]
        public int PageIndex { get; set; } = 0;
    }
}
