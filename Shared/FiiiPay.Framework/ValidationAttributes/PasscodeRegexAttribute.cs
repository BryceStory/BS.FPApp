using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FiiiPay.Framework.ValidationAttributes
{
    /// <summary>
    /// 密码规则验证，默认规则为8-20位字符串，以字母开头，须包含数字，不允许特殊字符
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PasscodeRegexAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasscodeRegexAttribute"/> class.
        /// </summary>
        public PasscodeRegexAttribute()
        {
        }

        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLength = 8;
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength = 20;
        /// <summary>
        /// 允许数字
        /// </summary>
        public bool AllowNumber = true;
        /// <summary>
        /// 允许特殊字符
        /// </summary>
        public bool AllowSpecialCharactr = false;
        /// <summary>
        /// 必须包含数字
        /// </summary>
        public bool NeedNumber = true;
        /// <summary>
        /// 必须包含特殊字符
        /// </summary>
        public bool NeedSpecialCharacter = false;

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified value is valid; otherwise, <see langword="false" />.
        /// </returns>
        public override bool IsValid(object value)
        {
            if (NeedNumber)
                AllowNumber = true;
            if (NeedSpecialCharacter)
                AllowSpecialCharactr = true;

            var regRequired = $"^.*(?=.{{{MinLength},{MaxLength}}})";
            if (NeedNumber)
                regRequired += "(?=.*\\d)";
            if (NeedSpecialCharacter)
                regRequired += "(?=.*[!@#$%^&*?])";
            regRequired += ".*$";
            
            if (!Regex.IsMatch(value.ToString(), regRequired))
                return false;
            
            if (!AllowNumber)
            {
                string regAllowed = @"^.*(?=\d+).*$";
                if(Regex.IsMatch(value.ToString(), regAllowed))
                    return false;
            }
            if (!AllowSpecialCharactr)
            {
                string regAllowed = @"^.*(?=[!@#$%^&*?]+).*$";
                if (Regex.IsMatch(value.ToString(), regAllowed))
                    return false;
            }
            
            return true;
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
