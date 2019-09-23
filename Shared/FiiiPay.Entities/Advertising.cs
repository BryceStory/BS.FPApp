using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class Advertising
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public LinkType LinkType { get; set; }
        public bool Status { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid PictureZh { get; set; }
        public Guid PictureEn { get; set; }
        public int Version { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
