namespace FiiiPay.Framework.MongoDB
{
    /// <summary>
    /// 消息实体
    /// </summary>
    public class Messages : MongoBaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户类型0=会员 1=商家
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// Gets or sets the notice identifier.
        /// </summary>
        /// <value>
        /// The notice identifier.
        /// </value>
        public string NoticeId { get; set; }

        /// <summary>
        /// 订单Id相关
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// 消息类型[1=收款成功 3=退款成功 5=充币成功 6=提币成功 7=提币失败 
        /// </summary>
        public int MsgType { get; set; }

        /// <summary>
        /// 国际化标题Key
        /// </summary>
        public string TitleKey { get; set; }

        /// <summary>
        /// 国际化标题SubKey
        /// </summary>
        public string SubTitleKey { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string CoinCode { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 0=未读 1=已读 2=已删除
        /// </summary>
        public int Status { get; set; }

        
    }
}
