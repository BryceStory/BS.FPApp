using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Common
{
    public class FileUploadModel
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public byte[] File { get; set; }
        public string FileType { get; set; }
    }
}