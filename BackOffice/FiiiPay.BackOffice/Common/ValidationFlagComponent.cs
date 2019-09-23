using FiiiPay.Framework.Enums;
using System;

namespace FiiiPay.BackOffice.Common
{
    public class ValidationFlagComponent
    {
        /// <summary>
        /// 判断是否开否某个密保方式
        /// </summary>
        /// <param name="validateValue"></param>
        /// <param name="validationFlag"></param>
        /// <returns></returns>
        public static bool CheckSecurityOpened(byte? validateValue, ValidationFlag validationFlag)
        {
            if (!validateValue.HasValue || validateValue.Value == 0)
                return false;
            ValidationFlag validate = (ValidationFlag)Enum.Parse(typeof(ValidationFlag), validateValue.Value.ToString());
            return (validate & validationFlag) != 0;
        }
        /// <summary>
        /// 开启某个密保方式，获得开启后的值
        /// </summary>
        /// <param name="oldValidateValue"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public static byte AddValidationFlag(byte? oldValidateValue, ValidationFlag toSet)
        {
            if (!oldValidateValue.HasValue || oldValidateValue.Value == 0)
                return (byte)toSet;
            ValidationFlag validate = (ValidationFlag)Enum.Parse(typeof(ValidationFlag), oldValidateValue.Value.ToString());
            return (byte)(validate | toSet);
        }
        /// <summary>
        /// 关闭某个密保方式，获得关闭后的值
        /// </summary>
        /// <param name="oldValidateValue"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public static byte ReduceValidationFlag(byte? oldValidateValue, ValidationFlag toSet)
        {
            if (!oldValidateValue.HasValue || oldValidateValue.Value == 0)
                return (byte)0;
            ValidationFlag validate = (ValidationFlag)Enum.Parse(typeof(ValidationFlag), oldValidateValue.Value.ToString());
            return (byte)(validate & (~toSet));
        }
    }
}