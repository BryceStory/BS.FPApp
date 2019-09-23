namespace FiiiPay.Framework
{
    /// <summary>
    /// Class ServiceResult
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// ServiceResult Constructor
        /// </summary>
        public ServiceResult() { }

        /// <summary>
        /// Gets or sets the reason code.
        /// </summary>
        /// <value>
        /// The reason code.
        /// </value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }

    /// <summary>
    /// Class ServiceResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ServiceResult" />
    public class ServiceResult<T> : ServiceResult
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; set; }
    }
}