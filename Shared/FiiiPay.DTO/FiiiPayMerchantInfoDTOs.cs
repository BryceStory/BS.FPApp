using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class FiiiPayMerchantInfoCreateIM
    {
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 营业时间描述
        /// </summary>
        public string WeekTxt { get; set; }
        /// <summary>
        /// 产品/服务
        /// </summary>
        public string[] TagList { get; set; }
        /// <summary>
        /// 商户类型
        /// </summary>
        public int[] MerchantCategorys { get; set; }
        /// <summary>
        /// 支持的收款币种
        /// </summary>
        public int[] SupportCoins { get; set; }
        /// <summary>
        /// 商户介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 门店所在国家Id
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// 省/州Id
        /// </summary>
        public long? StateId { get; set; }

        /// <summary>
        /// 市Id
        /// </summary>
        public long? CityId { get; set; }
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
        /// 申请人
        /// </summary>
        public string ApplicantName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 店面照片
        /// </summary>
        public Guid[] StorefrontImg { get; set; }
        /// <summary>
        /// 店铺环境图
        /// </summary>
        public Guid[][] FigureImgList { get; set; }
        /// <summary>
        /// 营业执照号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 营业执照图
        /// </summary>
        public Guid BusinessLicenseImage { get; set; }
        /// <summary>
        /// 是否使用FIII抵扣交易手续费
        /// </summary>
        public bool UseFiiiDeduction { get; set; }
    }
}
