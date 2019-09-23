using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.DTO
{
    public class BindNoticeRegIdIM
    {
        /// <summary>
        /// 注册极光推送服务得到的Id
        /// </summary>
        [Required]
        public string NoticeRegId { get; set; }
    }
}
