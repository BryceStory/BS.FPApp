using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Account
{
    public class BindNoticeRegIdIM
    {
        /// <summary>
        /// 注册极光推送服务得到的Id
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string NoticeRegId { get; set; }
    }
}
