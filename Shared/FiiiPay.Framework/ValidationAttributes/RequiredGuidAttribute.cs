using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Framework.ValidationAttributes
{
    /// <summary>
    /// Class FiiiPay.Framework.ValidationAttributes.RequiredGuidAttribute
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredGuidAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether [allow empty unique identifier].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow empty unique identifier]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowEmptyGuid { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.
        /// </returns>
        public override bool IsValid(object value)
        {
            Guid guidValue;
            if (value is Guid guid)
            {
                guidValue = guid;
            }
            else if (value is string stringValue)
            {
                if (!Guid.TryParse(stringValue, out guidValue))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return AllowEmptyGuid || guidValue != Guid.Empty;
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