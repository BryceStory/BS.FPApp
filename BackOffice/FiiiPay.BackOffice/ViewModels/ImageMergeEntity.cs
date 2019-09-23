using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class ImageMergeEntity
    {
        /// <summary>
        /// 类型
        /// </summary>
        public ImageContentType ContentType { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public Bitmap Image { get; set; }
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 文字字体
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// 文字大小
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// 文字颜色
        /// </summary>
        public Color FontColor { get; set; }
        /// <summary>
        /// 区域的宽度
        /// </summary>
        public float AreaWidth { get; set; }
        /// <summary>
        /// 区域的高度
        /// </summary>
        public float AreaHeight { get; set; }
        /// <summary>
        /// 位置横坐标
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// 位置纵坐标
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Index { get; set; }
    }
    public enum ImageContentType
    {
        Image,
        Text
    }
}
