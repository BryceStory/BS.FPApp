using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Profile.API.Models
{
    /// <summary>
    /// Class FiiiPay.Profile.API.Models.FileHasExistModel
    /// </summary>
    public class FileHasExistModel
    {
        /// <summary>
        /// Gets or sets the MD5.
        /// </summary>
        /// <value>
        /// The MD5.
        /// </value>
        [Required]
        public string Md5 { get; set; }
    }
}