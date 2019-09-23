using System;
using System.ComponentModel;

namespace FiiiPay.Foundation.Entities
{
    /// <summary>
    /// Represents a row of File data.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Gets or sets a Guid value for the Id column.
        /// </summary>
        [Browsable(false)]
        public Guid Id { get; set; }

        public string Md5 { get; set; }

        public string GroupName { get; set; }

        public string FileType { get; set; }
        
        public string FileName { get; set; }

        public string MimeType { get; set; }
        
        public string FilePath { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
