namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class ResultDto
    /// </summary>
    public class ResultDto
    {
        /// <summary>
        /// The result code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; set; }

        /// <summary>
        /// The result message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }

    /// <summary>
    /// Class ResultDto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultDto<T> : ResultDto
    {
        /// <summary>
        /// The result data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }
    }
}