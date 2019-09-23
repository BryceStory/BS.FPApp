using FiiiPay.Framework.Enums;
using System;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.Web.API.Models.Output
{
    /// <summary>
    /// 
    /// </summary>
    public class MerchantProfileOM
    {
        
        /// <summary>
        /// 商家Id
        /// </summary>
        public Guid? MerchantId { set; get; }

        /// <summary>
		/// 姓
		/// </summary>
		public string FirstName { set; get; }
        /// <summary>
		/// 名
		/// </summary>
		public string LastName { set; get; }
        /// <summary>
		/// 证件类型
		/// </summary>
		public IdentityDocType? IdentityDocType { set; get; }
        /// <summary>
		/// 证件号码
		/// </summary>
		public string IdentityDocNo { set; get; }

        /// <summary>
		/// 邮编
		/// </summary>
		public string Postcode { set; get; }

        /// <summary>
		/// 地址1
		/// </summary>
		public string Address1 { set; get; }
        /// <summary>
        /// 国家
        /// </summary>
        public int Country { set; get; }
        /// <summary>
        /// 州/省
        /// </summary>
        public string State { set; get; }
        /// <summary>
		/// 城市
		/// </summary>
		public string City { set; get; }
        /// <summary>
        /// 营业执照名称
        /// </summary>
        public string CompanyName { set; get; }

        /// <summary>
        /// 营业执照SN
        /// </summary>
        public string LicenseNo { set; get; }
 
        /// <summary>
		/// 商家个人信息状态 0=未认证 1=已认证 2=认证失败 3=审核中
		/// </summary>
		public VerifyStatus? L1VerifyStatus { set; get; }
        /// <summary>
        /// 营业执照状态 0=未认证 1=已认证 2=认证失败 3=审核中
        /// </summary>
        public VerifyStatus? L2VerifyStatus { set; get; }
        /// <summary>
        /// 审核失败的原因
        /// </summary>
        public string L1Remark { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string L2Remark { set; get; }

        /// <summary>
        /// 证件照（正面）
        /// </summary>
        public Guid? FrontIdentityImage { get; set; }
        /// <summary>
        /// 证件照（反面）
        /// </summary>
        public Guid? BackIdentityImage { get; set; }
        /// <summary>
        /// 证件照（手持）
        /// </summary>
        public Guid? HandHoldWithCard { get; set; }
        /// <summary>
        /// 营业执照图片Id
        /// </summary>
        public Guid? BusinessLicenseImage { set; get; }
    }
}