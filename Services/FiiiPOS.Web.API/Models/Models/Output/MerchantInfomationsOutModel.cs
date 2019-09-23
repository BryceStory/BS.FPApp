using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPOS.Web.API.Models.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPOS.Web.API.Models.Models.Output
{
    /// <summary>
    /// 传出门店信息
    /// </summary>
    public class MerchantInfomationsOutModel
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
        public List<int> Week { get; set; }

        /// <summary>
        /// 营业开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 营业结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 产品/服务
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

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
        /// 商家审核状态
        /// </summary>
        public VerifyStatus VerifyStatus { get; set; }

        /// <summary>
        /// 商家停用状态
        /// </summary>
        public Status Status { get; set; }

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
        public List<FiiiPay.Entities.Recommend> Recommends { get; set; }

        /// <summary>
        /// 国家信息
        /// </summary>
        public Countrys Countrys { get; set; }

        public Status IsPublic { get; set; }

    }
}
