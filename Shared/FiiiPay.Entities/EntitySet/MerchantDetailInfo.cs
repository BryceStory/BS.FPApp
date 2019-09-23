using System;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantDetailES
    {
        /// <summary>
		/// 商户Id
		/// </summary>
		public Guid Id { set; get; }

        /// <summary>
        /// 商户账号
        /// </summary>
        public string Username { set; get; }

        /// <summary>
        /// POS序列号
        /// </summary>
        public long POSId { set; get; }

        /// <summary>
        /// 商户手机
        /// </summary>
        public string Cellphone { set; get; }

        /// <summary>
		/// 所在国家
		/// </summary>
		public int Country { set; get; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { set; get; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { set; get; }

        /// <summary>
		/// 所在城市
		/// </summary>
		public int City { set; get; }

        /// <summary>
		/// 所在城市
		/// </summary>
		public string CityName { set; get; }

        /// <summary>
        /// 所在州
        /// </summary>
        public int State { set; get; }

        /// <summary>
		/// 邮编
		/// </summary>
		public string Postcode { set; get; }

        /// <summary>
		/// 地址1
		/// </summary>
		public string Address1 { set; get; }

        /// <summary>
        /// 地址2
        /// </summary>
        public string Address2 { set; get; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { set; get; }


        /// <summary>
		/// 营业执照图片Id
		/// </summary>
		public Guid BusinessLicenseImage { set; get; }

        /// <summary>
        /// 营业执照名称
        /// </summary>
        public string CompanyName { set; get; }

        /// <summary>
        /// 营业执照SN
        /// </summary>
        public string LicenseNo { set; get; }

        /// <summary>
        /// 审核失败的原因
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
		/// 营业执照状态
		/// </summary>
		public int VerifyStatus { set; get; }

    }
}
