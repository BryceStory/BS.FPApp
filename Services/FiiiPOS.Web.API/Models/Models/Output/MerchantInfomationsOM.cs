using FiiiPOS.Web.API.Models.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Models.Output
{
    public class MerchantInfomationsOM
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户类别
        /// </summary>
        public List<int> Categorys { get; set; }

        /// <summary>
        /// 营业日期
        /// </summary>
        public int Week { get; set; }

        /// <summary>
        /// 营业开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 产品/服务
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public int Phone { get; set; }

        /// <summary>
        /// 商家介绍
        /// </summary>
        public string Introduce { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Lat { get; set; }

        /// <summary>
        /// 门店主图列表
        /// </summary>
        public List<Guid> OwnersFigures { get; set; }

        /// <summary>
        /// 商家推荐图列表
        /// </summary>
        public List<Recommend> Recommends { get; set; }

    }
}