using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Profile.API.Models
{
    /// <summary>
    /// Class FiiiPay.Profile.API.Models.FileUploadModel
    /// </summary>
    public class FileUploadModel
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [Required]
        public byte[] File { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>
        /// The type of the file.
        /// </value>
        public string FileType { get; set; }
    }
}