using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class FileUploadModel
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public byte[] File { get; set; }
        public string FileType { get; set; }
    }
    public class FileUploadWithThumbnailModel
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public byte[] File { get; set; }
        public string FileType { get; set; }
    }
}