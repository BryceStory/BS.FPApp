using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Invite
{
    /// <summary>
    /// 邀请排行榜
    /// </summary>
    public class InviteRankIM
    {
        /// <summary>
        /// 0:fiiipay 1:fiiipos
        /// </summary>
        [Required]
        public int Type { get; set; }

        /// <summary>
        /// 显示排行榜条数 默认为金银铜三个
        /// </summary>
        [Required]
        public int Count { get; set; } = 3;
    }
}
