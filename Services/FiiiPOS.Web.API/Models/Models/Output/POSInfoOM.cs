using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Output
{
    public class POSInfoOM
    {
        /// <summary>
        /// POS机序列号
        /// </summary>
        public string POSSN { set; get; }
        /// <summary>
        /// 型号
        /// </summary>
        public string ModelNumber { set; get; }
        /// <summary>
        /// 处理器信息
        /// </summary>
        public string ProcessorInfo { set; get; }
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public string ResolutionRatio { set; get; }
        /// <summary>
        /// Android版本
        /// </summary>
        public string AndroidVersion { set; get; }
        /// <summary>
        /// 固件版本号
        /// </summary>
        public string FirmwareVersion { set; get; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        public DateTime FactoryDate { set; get; }
        /// <summary>
        /// 基带版本
        /// </summary>
        public string BasebandVersion { set; get; }
    }
}