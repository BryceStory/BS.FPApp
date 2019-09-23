using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Invite
{
    public class SingleBonusDetailIM
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        [Required, Plus]
        public int Id { get; set; }
    }
}
