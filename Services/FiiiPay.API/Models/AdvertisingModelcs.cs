using System;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// 闪屏广告信息
    /// </summary>
    public class AdvertisingModelcs
    {
        /// <summary>
        /// 广告唯一标志
        /// </summary>
        public long SingleId { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 是否由APP打开
        /// </summary>
        public bool OpenByAPP { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 英文图片ID
        /// </summary>
        public Guid ImageId { get; set; }
        /// <summary>
        /// 中文图片ID
        /// </summary>
        public Guid CNImageId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Status { get; set; }
    }
    
    public class StoreBannerOM
    {
        /// <summary>
        /// 是否由APP打开
        /// </summary>
        public bool OpenByAPP { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 查看所需权限 0无需权限 1需登录 2需KYC通过
        /// </summary>
        public byte ViewPermission { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片ID
        /// </summary>
        public Guid ImageId { get; set; }
    }
}