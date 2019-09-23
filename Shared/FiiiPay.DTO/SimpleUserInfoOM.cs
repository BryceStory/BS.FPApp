using System;

namespace FiiiPay.DTO.Account
{
    public class SimpleUserInfoOM
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 已经加上*
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 用户手机号，已经带上地区码和加上***，比如 86 *******1200
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 用户头像，客户端需要拼接地址下载图片
        /// </summary>
        public Guid? Avatar { get; set; }

        /// <summary>
        /// 是否已经设置过PIN码
        /// </summary>
        public bool HasSetPin { get; set; }

        /// <summary>
        /// 是否完善了个人基本信息
        /// </summary>
        public bool IsBaseProfileComplated { get; set; }

        /// <summary>
        /// 是否通过了KYC审核
        /// </summary>
        public bool IsLV1Verified { get; set; }

        /// <summary>
        /// 用来生成随机码的Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 法币id
        /// </summary>
        public int FiatId { get; set; }
        /// <summary>
        /// 法币简称
        /// </summary>
        public string FiatCode { get; set; }
    }
}
