using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class StoreBanner
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public BannerStatus Status { get; set; }
        public int CountryId { get; set; }
        public int Sort { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BannerPermission ViewPermission { get; set; }
        public string Title { get; set; }
        public Guid PictureId { get; set; }
        public string LinkUrl { get; set; }
        public bool OpenByAPP { get; set; }
    }

    public enum BannerStatus
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Closed = 0,
        /// <summary>
        /// 正常
        /// </summary>
        Active
    }
    public enum BannerPermission : int
    {
        NoNeed = 0,
        NeedLogin,
        NeedKYC
    }
}
