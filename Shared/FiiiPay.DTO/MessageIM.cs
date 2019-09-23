using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadMessageIM
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        [Required,MongoId]
        public string Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DeleteMessageIM
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        [Required,MongoId]
        public string Id { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class MessageListIM
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, Plus]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 从0开始
        /// </summary>
        [Required, Plus(true)]
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 最大可以查询类型值
        /// </summary>
        public int? MaxType { get; set; }
    }

    /// <summary>
    /// 测试准用
    /// </summary>
    public class AddMessageIM
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; }

        /// <summary>
        /// 消息类型[推送类型]
        /// </summary>
        [Required]
        public int MsgType { get; set; }

        /// <summary>
        /// 订单Id相关
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// 国际化标题Key
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string TitleKey { get; set; }

        /// <summary>
        /// 国际化标题SubKey
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string SubTitleKey { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        [Required(AllowEmptyStrings = false),MaxLength(50)]
        public string CoinCode { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Body { get; set; }

    }
}
