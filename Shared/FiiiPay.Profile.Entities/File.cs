using System;
using System.ComponentModel;

namespace FiiiPay.Profile.Entities
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

        /// <summary>
        /// Gets or sets a long value for the AccountId column.
        /// </summary>

        public Guid? AccountId { get; set; }

        /// <summary>
        /// Gets or sets a string value for the FileType column.
        /// </summary>

        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets a string value for the FileName column.
        /// </summary>

        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a string value for the MimeType column.
        /// </summary>

        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets a string value for the FilePath column.
        /// </summary>

        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a DateTime value for the Timestamp column.
        /// </summary>

        public DateTime Timestamp { get; set; }
    }
}
