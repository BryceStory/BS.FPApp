namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.ExtensionString
    /// </summary>
    public static class ExtensionString
    {
        /// <summary>
        /// Hides the primary information.
        /// </summary>
        /// <param name="OldString">The old string.</param>
        /// <param name="startLen">The start length.</param>
        /// <param name="endLen">The end length.</param>
        /// <param name="specialChar">The special character.</param>
        /// <returns></returns>
        public static string HidePrimaryInfo(this string OldString, int startLen, int endLen, char specialChar)
        {
            if (OldString.Length <= 4)
            {
                startLen = endLen = 1;
            }

            var lenth = OldString.Length - startLen - endLen;
            var replaceStr = OldString.Substring(startLen, lenth);
            var specialStr = string.Empty;
            for (var i = 0; i < replaceStr.Length; i++)
            {
                specialStr += specialChar;
            }

            return OldString.Replace(replaceStr, specialStr);
        }

        /// <summary>
        /// Hides the primary string.
        /// </summary>
        /// <param name="oldString">The old string.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="specialChar">The special character.</param>
        /// <returns></returns>
        public static string HidePrimaryString(this string oldString, int start, int end, char specialChar)
        {
            var specialLength = oldString.Length - start - end;
            if (specialLength <= 0)
            {
                return oldString;
            }

            var beginStr = oldString.Substring(0, start);
            var endStr = oldString.Substring(oldString.Length - end, end);

            var newStr = beginStr.PadRight(specialLength, specialChar) + endStr;
            return newStr;
        }

        /// <summary>
        /// Hides the primary information.
        /// </summary>
        /// <param name="OldString">The old string.</param>
        /// <param name="lenth">The lenth.</param>
        /// <param name="specialChar">The special character.</param>
        /// <returns></returns>
        public static string HidePrimaryInfo(this string OldString, int lenth, char specialChar)
        {
            var startLen = OldString.Length / 2 - lenth / 2;
            var endLen = OldString.Length - startLen - lenth;
            var strlenth = OldString.Length - startLen - endLen;
            var replaceStr = OldString.Substring(startLen, lenth);
            var specialStr = string.Empty;
            for (var i = 0; i < replaceStr.Length; i++)
            {
                specialStr += specialChar;
            }

            return OldString.Replace(replaceStr, specialStr);
        }

        /// <summary>
        /// Hides the primary information by lenth.
        /// </summary>
        /// <param name="OldString">The old string.</param>
        /// <param name="startCount">The start count.</param>
        /// <param name="specialChar">The special character.</param>
        /// <returns></returns>
        public static string HidePrimaryInfoByLenth(this string OldString, int startCount, char specialChar)
        {
            var CutCount = 2;

            if (OldString.Length < 5)
            {
                CutCount = 1;
            }

            var newString = OldString.Substring(0, CutCount);
            for (var i = 0; i < startCount; i++)
            {
                newString += specialChar;
            }

            newString += OldString.Substring(OldString.Length - CutCount);

            return newString;
        }
    }
}
