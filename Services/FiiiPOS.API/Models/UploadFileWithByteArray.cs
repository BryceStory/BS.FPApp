using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.API.Models
{
    public class UploadFileWithByteArray
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public byte[] Stream { get; set; }
    }
}