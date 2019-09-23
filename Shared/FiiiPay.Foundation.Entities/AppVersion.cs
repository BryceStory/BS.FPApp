namespace FiiiPay.Foundation.Entities
{
    public class AppVersion
    {
        public int Id { get; set; }

        /// <summary>
        /// 0：ios，1：android
        /// </summary>
        public PlatfromEmum Platform { get; set; }

        /// <summary>
        /// IOS企业版/安卓谷歌商店版
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///  IOS企业版/安卓谷歌商店版是否强制更新
        /// </summary>
        public bool ForceToUpdate { get; set; }

        /// <summary>
        /// IOS企业版/安卓谷歌商店版下载地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 应用，0：FiiiPay APP，1：FiiiPos
        /// 后面新增APP后继续这样添加即可
        /// </summary>
        public AppEnum App { get; set; }

        /// <summary>
        /// 数据格式：{"zh":"text1|||text2","en":"text1|||text2"}
        /// </summary>
        public string Description { get; set; }
    }

    public enum PlatfromEmum : byte
    {
        iOS = 0,
        Android,
        iOSEnterprise,
        AndroidEnterprise
    }

    public enum AppEnum : byte
    {
        FiiiPay = 0,
        FiiiPOS
    }
}
