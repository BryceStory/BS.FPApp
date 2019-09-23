using System;

namespace FiiiPay.DTO.Profile
{
    public class InfoOM
    {
        /// <summary>
        /// 已经加上*
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 字符串，客户端直接显示即可
        /// </summary>
        public string VerifiedStatus { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public Guid? Avatar { get; set; }

        public string CountryName { get; set; }

        /// <summary>
        /// 用户手机号，已经带上地区码和加上***，比如 86 *******1200
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 已经加上****
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 1：男，0：女
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 生日，Unix时间戳
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        public string Nickname { get; set; }
    }
}
