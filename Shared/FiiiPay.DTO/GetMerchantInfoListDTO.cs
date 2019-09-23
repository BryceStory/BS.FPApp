using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.DTO
{
    public class GetMerchantInfoListIM
    {
        public Location Location { get; set; }

        /// <summary>
        /// 数据条数 默认十条
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 从0开始
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 筛选
        /// </summary>
        public SearchFilter Filter { get; set; }

        /// <summary>
        /// 搜索筛选类
        /// </summary>
        public sealed class SearchFilter
        {
            /// <summary>
            /// 关键词搜索
            /// </summary>
            public string KeyWord { get; set; }

            /// <summary>
            /// 通过国家筛选
            /// </summary>
            [Required]
            public int CountryId { get; set; }

            /// <summary>
            /// 类别搜索 默认为-1 则是搜索全部
            /// </summary>
            public int Category { get; set; } = -1;

            /// <summary>
            /// 距离范围
            /// </summary>
            public decimal Distance { get; set; } = 50000;
        }
    }
    /// <summary>
    /// 坐标信息类
    /// </summary>
    public sealed class Location
    {
        /// <summary>
        /// 经度坐标
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度坐标
        /// </summary>
        public decimal Latitude { get; set; }
    }
    

    public class GetMerchantInfoOM
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 门店类型 1Fiiipay门店 2FiiiPos门店
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// 距离 单位m
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 可接受的币种列表
        /// </summary>
        public List<string> AvailableCryptoIconList { get; set; }

        /// <summary>
        /// 特色标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 原图下载地址
        /// </summary>
        public Guid OriginIconId { get; set; }

        /// <summary>
        /// 缩略图下载地址
        /// </summary>
        public Guid CompressIconId { get; set; }

        /// <summary>
        /// 是否允许消费
        /// </summary>
        public bool IsAllowExpense { get; set; }

        /// <summary>
        /// 商店名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 位置信息
        /// </summary>
        public Location Location { get; set; }
    }
    
}