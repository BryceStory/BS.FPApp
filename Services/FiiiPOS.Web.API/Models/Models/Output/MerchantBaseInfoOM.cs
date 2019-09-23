using System;

namespace FiiiPOS.Web.API.Models.Output
{
    /// <summary>
    /// 
    /// </summary>
    public class MerchantBaseInfoOM
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
        /// POS机序列号
        /// </summary>
        public string PosSN { set; get; }

        /// <summary>
        /// 商户手机
        /// </summary>
        public string Cellphone { set; get; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { set; get; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName_CN { set; get; }


        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { set; get; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { set; get; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { set; get; }
        
        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        public int IsVerifiedEmail { set; get; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviterCode { set; get; }

        public bool IsExistMerchantInfo { get; set; }

    }
}