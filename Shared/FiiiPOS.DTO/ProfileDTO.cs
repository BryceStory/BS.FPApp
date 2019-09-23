using System;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class ProfileDTO
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        public string LastName { set; get; }
        /// <summary>
        /// 姓
        /// </summary>
        public string FirstName { set; get; }
        /// <summary>
        /// 身份号码
        /// </summary>
        public string IdentityDocNo { set; get; }
        /// <summary>
        /// 护照/身份证
        /// </summary>
        public IdentityDocType? IdentityDocType { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? FrontIdentityImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? BackIdentityImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? HandHoldWithCard { get; set; }

        /// <summary>
        /// 营业执照认证状态
        /// Uncertified=0,Certified=1,Disapproval=2,UnderApproval=3
        /// </summary>
        public int L1VerifyStatus { get; set; }
        /// <summary>
        /// 商家个人信息认证状态
        /// </summary>
        public int L2VerifyStatus { get; set; }
        /// <summary>
        /// 商家帐号
        /// </summary>
        public string MerchantAccount { get; set; }

        /// <summary>
        /// 营业执照名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// POS机序列号
        /// </summary>
        public string PosSn { get; set; }
        /// <summary>
        /// 地址1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// 地址2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Postcode { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string State { set; get; }
    }
}