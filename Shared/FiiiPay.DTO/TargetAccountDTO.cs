using System;

namespace FiiiPay.DTO.Investor
{
    /// <summary>
    /// TargetAccountDTO
    /// </summary>
    public class TargetAccountDTO
    {
        /// <summary>
        /// FullName,已经加上*
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 用户头像Id，客户端需要拼接地址下载图片
        /// </summary>
        public Guid? Avatar { get; set; }

    }
}