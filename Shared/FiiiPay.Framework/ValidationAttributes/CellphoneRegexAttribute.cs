using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Framework.ValidationAttributes
{
    /// <summary>
    /// Class FiiiPay.Framework.ValidationAttributes.CellphoneRegexAttribute
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CellphoneRegexAttribute : ValidationAttribute
    {
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.
        /// </returns>
        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value.ToString())) return false;

            var reg = "^\\d{3,50}$";
            return System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), reg);
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
            return $"Invalid {name} format.";
        }
    }
}
