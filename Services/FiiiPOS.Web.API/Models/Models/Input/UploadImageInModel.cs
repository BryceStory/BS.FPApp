using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadImageInModel
    {
        /// <summary>
        /// 文件名，带后缀
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Base64Content { get; set; }
    }
}