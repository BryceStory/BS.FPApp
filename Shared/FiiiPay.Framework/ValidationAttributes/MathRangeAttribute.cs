using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Framework.ValidationAttributes
{
    /// <summary>
    /// Class FiiiPay.Framework.ValidationAttributes.MathRangeAttribute
    /// </summary>
    /// <seealso cref="System.ComponentModel.DataAnnotations.RangeAttribute" />
    public class MathRangeAttribute : RangeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathRangeAttribute"/> class.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public MathRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathRangeAttribute"/> class.
        /// </summary>
        /// <param name="type">Specifies the type of the object to test.</param>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public MathRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathRangeAttribute"/> class.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        public MathRangeAttribute(long minimum, long maximum)
            : base(typeof(long), minimum.ToString(), maximum.ToString())
        {

        }
    }
}