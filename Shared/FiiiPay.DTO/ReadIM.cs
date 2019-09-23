using FiiiPay.Entities;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Article
{
    public class ReadIM
    {
        /// <summary>
        /// 消息的Id，要与Type一起才能确定唯一性
        /// </summary>
        [Required(AllowEmptyStrings = false),MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [Required]
        public ReadRecordType? Type { get; set; }
    }
}
