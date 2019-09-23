using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.Foundation.API.Models
{
    public class UploadWithCompressOM
    {
        /// <summary>
        /// 原图的id
        /// </summary>
        public Guid ArtWork { get; set; }

        /// <summary>
        /// 缩略图id
        /// </summary>
        public Guid Thumbnail { get; set; }
    }
}