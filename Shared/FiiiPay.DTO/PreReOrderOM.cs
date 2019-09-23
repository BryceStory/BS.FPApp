using System;
using System.Collections.Generic;

namespace FiiiPay.DTO.HomePage
{
    public class PreReOrder1OM
    {
        public List<PreReOrder1OMItem> List { get; set; }
    }

    public class PreReOrder1OMItem
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 图标的地址，客户端需要拼接地址下载图片
        /// </summary>
        public Guid? IconUrl { get; set; }

        /// <summary>
        /// 币的简称，比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 币的名称，比如：Bitcoin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否固定到顶端
        /// </summary>
        public bool Fixed { get; set; }

        /// <summary>
        /// 是否显示在首页，客户端根据“不显示在首页”放在“更多币种”
        /// </summary>
        public bool ShowInHomePage { get; set; }
    }
}
