using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Framework.ValidationAttributes
{
    /// <summary>
    /// Class FiiiPay.Framework.ValidationAttributes.PlusAttribute
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PlusAttribute : ValidationAttribute
    {
        private readonly bool _isContainsZero;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlusAttribute"/> class.
        /// </summary>
        public PlusAttribute():this(false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isContainsZero">如果为true则是大于等于零</param>
        public PlusAttribute(bool isContainsZero)
        {
            _isContainsZero = isContainsZero;
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.
        /// </returns>
        public override bool IsValid(object value)
        {
            var v = value as decimal?;
            if (v == null)
            {
                v = value as int?;
                return _isContainsZero ? v >= 0 : v > 0;
            }
            return _isContainsZero ? v >= 0M : v > 0M;
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            return $"The {name} must be greater than zero.";
        }
    }
}