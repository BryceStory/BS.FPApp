using System.Collections.Generic;

namespace FiiiPay.DTO.AppVersion
{
    public class VersionOM
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool ForceToUpdate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public List<string> Description { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// api地址
        /// </summary>
        public string ApiAddress { get; set; }
    }
}
