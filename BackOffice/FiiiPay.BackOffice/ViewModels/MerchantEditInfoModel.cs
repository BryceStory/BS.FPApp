using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class MerchantEditInfoModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 营业时间描述(FiiiPay门店使用)
        /// </summary>
        public string WeekTxt { get; set; }
        /// <summary>
        /// 产品/服务
        /// </summary>
        public string[] TagList { get; set; }
        public int[] MerchantCategorys { get; set; }
        /// <summary>
        /// 支持的收款币种
        /// </summary>
        public int[] SupportCoins { get; set; }
        /// <summary>
        /// 商家介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 所在国家ID
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// Fiiipay账号
        /// </summary>
        public string FiiiPayAccount { get; set; }
        /// <summary>
        /// 门店地址所在国家编号
        /// </summary>
        public string CountryCode { get; set; }
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
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        public Guid FileId { get; set; }
        public string ApplicantName { get; set; }
        public Guid[] FigureImgIdList { get; set; }
        public string LicenseNo { get; set; }
        public Guid BusinessLicenseImage { get; set; }
    }
}