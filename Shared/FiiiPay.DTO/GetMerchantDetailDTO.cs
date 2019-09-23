using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.DTO
{
    public class GetMerchantDetailIM
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public Guid Id { get; set; }
    }

    public class GetMerchantDetailOM
    {
        /// <summary>
        /// 门店类型 1Fiiipay门店 2FiiiPos门店
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 轮播图片地址
        /// </summary>
        public IList<Guid> ImageUrls { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// 营业开始时间
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 营业星期 返回1就代表星期一
        /// </summary>
        public List<int> Weeks { get; set; }

        /// <summary>
        /// 营业时间文本说明，FiiiPay门店显示此文本
        /// </summary>
        public string BussinessHour { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 商户坐标
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// 支持的币种
        /// </summary>
        public IList<string> AvailableCryptoCodeList { get; set; }

        /// <summary>
        /// 支持的币种图标
        /// </summary>
        public IList<string> AvailableCryptoIconList { get; set; }

        /// <summary>
        /// 推荐商品列表
        /// </summary>
        public IList<Goods> RecommendGoods { get; set; }

        /// <summary>
        /// 商家介绍
        /// </summary>
        public string Introduce { get; set; }

        /// <summary>
        /// 商家联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 手机区号
        /// </summary>
        public string PhoneCode { get; set; }

        /// <summary>
        /// 是否允许消费
        /// </summary>
        public bool IsAllowExpense { get; set; }

    }
    /// <summary>
    /// 商品类
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 商品图片
        /// </summary>
        public ImageWithCompressImage Image { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Content { get; set; }
    }
    /// <summary>
    /// 原图和缩略图
    /// </summary>
    public class ImageWithCompressImage
    {
        /// <summary>
        /// 原图
        /// </summary>
        public Guid Origin { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public Guid Compress { get; set; }
    }
}