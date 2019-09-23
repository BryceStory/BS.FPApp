using System;

namespace FiiiPay.Framework.Exceptions
{
    /// <summary>
    /// Class FiiiPay.Framework.Exceptions.CommonException
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CommonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonException"/> class.
        /// </summary>
        public CommonException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonException"/> class.
        /// </summary>
        /// <param name="reasoncode">The reasoncode.</param>
        /// <param name="message">The message.</param>
        public CommonException(int reasoncode, string message) : base(message)
        {
            ReasonCode = reasoncode;
        }

        /// <summary>
        /// Gets or sets the reason code.
        /// </summary>
        /// <value>
        /// The reason code.
        /// </value>
        public int ReasonCode { get; set; }
    }
}
