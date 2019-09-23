using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 上传图片model
    /// </summary>
    public class UploadImageModel
    {
        /// <summary>
        /// 文件名，带后缀
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 图片的Base64编码
        /// </summary>
        [Required]
        public string Base64Content { get; set; }
    }
}
