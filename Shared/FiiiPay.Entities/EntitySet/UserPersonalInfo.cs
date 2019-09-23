using System;

namespace FiiiPay.Entities.EntitySet
{
    public class UserPersonalInfo
    {
        public Guid? UserAccountId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; } 
        public string IdentityDocNo { get; set; }
        /// <summary>
        ///  是否认证
        /// </summary>
        public bool? IsIdentityDocVerified { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public byte? Gender { get; set; }
        public string Cellphone { get; set; }
        public int Country { get; set; }
        /// <summary>
        /// 头像存储地址
        /// </summary>
        public Guid? Avator { get; set; }
        public string Email { get; set; }
    }
}
