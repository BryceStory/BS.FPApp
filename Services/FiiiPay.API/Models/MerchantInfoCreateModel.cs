using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// 发送注册手机验证码
    /// </summary>
    public class MerchantCreateSMSCodeModel
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, Range(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class MerchantInfoCreateModel
    {
        /// <summary>
        /// 邀请码
        /// </summary>
        [StringLength(10)]
        public string InviteCode { get; set; }
        
        /// <summary>
        /// 商户名称
        /// </summary>
        [Required(AllowEmptyStrings =false),StringLength(100)]
        public string MerchantName { get; set; }
        /// <summary>
        /// 营业时间描述
        /// </summary>
        [Required(AllowEmptyStrings = false), StringLength(300)]
        public string WeekTxt { get; set; }
        /// <summary>
        /// 产品/服务
        /// </summary>
        [Required]
        public string[] TagList { get; set; }
        /// <summary>
        /// 商户类型
        /// </summary>
        [Required]
        public int[] MerchantCategorys { get; set; }
        /// <summary>
        /// 支持的收款币种
        /// </summary>
        [Required]
        public int[] SupportCoins { get; set; }
        /// <summary>
        /// 商户介绍
        /// </summary>
        [StringLength(4000)]
        public string Introduce { get; set; }

        /// <summary>
        /// 国家Id
        /// </summary>
        [Required]
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
        [Required(AllowEmptyStrings = false), StringLength(100)]
        public string Address { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [Required,Range(-180,180)]
        public decimal Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [Required, Range(-90, 90)]
        public decimal Lat { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplicantName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [Required(AllowEmptyStrings = false), StringLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// 店面照片
        /// </summary>
        [Required]
        public Guid[] StorefrontImg { get; set; }
        /// <summary>
        /// 店铺环境图
        /// </summary>
        [Required]
        public Guid[][] FigureImgList { get; set; }
        /// <summary>
        /// 营业执照号
        /// </summary>
        [Required]
        public string LicenseNo { get; set; }
        /// <summary>
        /// 营业执照图
        /// </summary>
        [Required]
        public Guid BusinessLicenseImage { get; set; }

        /// <summary>
        /// 是否使用FIII抵扣交易手续费
        /// </summary>
        [Required]
        public bool UseFiiiDeduction { get; set; }
    }
}