namespace FiiiPOS.DTO.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadMessageIM
    {
        /// <summary>
        /// 消息Id
        /// </summary>
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
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 从0开始
        /// </summary>
        public int PageIndex { get; set; } = 0;
    }

}
