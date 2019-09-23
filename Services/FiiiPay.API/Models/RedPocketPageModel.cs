using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    public class RedPocketPageModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        [Required]
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [Required]
        public int PageSize { get; set; }
    }
}