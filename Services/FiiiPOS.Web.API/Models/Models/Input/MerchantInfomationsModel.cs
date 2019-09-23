using FiiiPOS.Web.API.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Models.Input
{
    /// <summary>
    /// 门店信息
    /// </summary>
    public class MerchantInfomationsModel
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(220)]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户类别
        /// </summary>
        [CollectionMaxCount(3)]
        public List<int> Categorys { get; set; }

        /// <summary>
        /// 营业日期
        /// </summary>
        [Required]
        public List<int> Week { get; set; }

        /// <summary>
        /// 营业开始时间
        /// </summary>
        [Required]
        public string StartTime { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        [Required]
        public string EndTime { get; set; }

        /// <summary>
        /// 产品/服务
        /// </summary>
        [Required]
        public List<string> Tags { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// 商家介绍
        /// </summary>
        [MaxLength(4000)]
        public string Introduce { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(300)]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Required]
        public decimal Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Required,Range(-90,90)]
        public decimal Lat { get; set; }

        /// <summary>
        /// 门店主图列表
        /// </summary>
        [Required]
        public List<Guid> OwnersFigures { get; set; }

        /// <summary>
        /// 商家推荐图列表
        /// </summary>
        [Required]
        public List<Recommend> Recommends { get; set; }

    }
    /// <summary>
    /// 商家推荐主图
    /// </summary>
    public class Recommend
    {
        /// <summary>
        /// 推荐主图文字
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(28)]
        public string Content { get; set; }

        /// <summary>
        /// 推荐主图ID
        /// </summary>
        [Required]
        public Guid Picture { get; set; }
    }

    public class StoreTypeOutModel
    {
        public int Id { get; set; }
        public string Name_CN { get; set; }
        public string Name_EN { get; set; }
    }
}