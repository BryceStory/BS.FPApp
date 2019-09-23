using System;

namespace FiiiPOS.Web.API.Models.Output
{
    /// <summary>
    /// 获取商家简单信息
    /// </summary>
    public class MerchantSimpleInfoOutModel
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
        /// 商户名称
        /// </summary>
        public string MerchantName { set; get; }

        /// <summary>
        /// 头像Id
        /// </summary>
        public string PhotoId { set; get; }


        
    }
}