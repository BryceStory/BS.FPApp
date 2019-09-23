using System;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class InvalidSystemClockException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    internal class InvalidSystemClockException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSystemClockException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidSystemClockException(string message) : base(message)
		{
        }
    }
}
