using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.DecimalExtension
    /// </summary>
    public static class DecimalExtension
    {

        /// <summary>
        /// 最少保留位数（不够添零），最多保留位数（位数过多舍去后面小数）
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="decimalPoint">The decimal point.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString(this decimal val, int minDecimalPoint, int maxDecimalPoint)
        {
            var value = val.ToSpecificDecimal(maxDecimalPoint);
            var items = new[] { minDecimalPoint, maxDecimalPoint, BitConverter.GetBytes(decimal.GetBits(value)[3])[2] };
            Array.Sort(items);
            return value.ToString($"F{items[items.Length/2]}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="decimalPoint">The decimal point.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString(this decimal val, int decimalPoint)
        {
            return val.ToSpecificDecimal(decimalPoint).ToString($"F{decimalPoint}", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// To the specific decimal.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="decimalPoint">The decimal point.</param>
        /// <returns></returns>
        public static decimal ToSpecificDecimal(this decimal val, int decimalPoint)
        {
            var pow = (int)Math.Pow(10, decimalPoint);
            return Math.Truncate(val * pow) / pow;
        }

        /// <summary>
        /// 截取为字符串
        /// </summary>
        /// <param name="num"></param>
        /// <param name="minPlace">最小小数位</param>
        /// <param name="maxPlace">最大小数位</param>
        /// <returns></returns>
        public static string ToString(this decimal num, ushort minPlace, ushort maxPlace)
        {
            long pow1 = (long)Math.Pow(10, maxPlace);
            decimal num1 = num * pow1;
            decimal floorNum1 = Math.Floor(num1);
            if (num1 > floorNum1)
                num = floorNum1 / pow1;

            return num.ToString(GetFormat(minPlace, maxPlace));
        }

        private static string GetFormat(ushort minPlace, ushort maxPlace)
        {
            string format = "0.";
            for (int i = 0; i < minPlace; i++)
                format += "0";

            for (int i = 0; i < maxPlace - minPlace; i++)
                format += "#";

            return format;
        }
    }
}