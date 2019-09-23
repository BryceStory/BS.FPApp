using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Image
{
    public class UploadImageIM
    {
        /// <summary>
        /// 文件名，带后缀
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// 图片的Base64编码
        /// </summary>
        [Required]
        public string Base64Content { get; set; }
    }

    public class UploadImageWithStreamIM
    {
        /// <summary>
        /// 文件名，带后缀
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// 文件byte数组
        /// </summary>
        [Required]
        public byte[] Stream { get; set; }
    }
}
