using System;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.TimeHelper
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// 转化为yyyyMMddHHmmssfff的格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns>yyyyMMddHHmmssfff</returns>
        public static string ConvertDateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 从yyyyMMddHHmmssfff格式的时间戳转化为时间
        /// </summary>
        /// <param name="timeStamp">yyyyMMddHHmmssfff</param>
        /// <returns></returns>
        public static DateTime ConvertTimeStampToDateTime(string timeStamp)
        {
            var yyyy = timeStamp.Substring(0, 4);
            var MM = timeStamp.Substring(4, 2);
            var dd = timeStamp.Substring(6, 2);
            var HH = timeStamp.Substring(8, 2);
            var mm = timeStamp.Substring(10, 2);
            var ss = timeStamp.Substring(12, 2);
            var fff = timeStamp.Substring(14, 3);

            var dt = new DateTime(
                Convert.ToInt32(yyyy), Convert.ToInt32(MM), Convert.ToInt32(dd),
                Convert.ToInt32(HH), Convert.ToInt32(mm), Convert.ToInt32(ss),
                Convert.ToInt32(fff)
                );

            return dt;
        }

        /// <summary>
        /// 距离1970-01-01的毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetUnixTime(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// To the unix time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime dt)
        {
            return dt.ToUtcTimeTicks();
        }

        /// <summary>
        /// To the unix time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static long ToUtcTimeTicks(this DateTime dt)
        {
            return (dt.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="dtStr">The dt string.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string dtStr)
        {
            var dtLong = Convert.ToInt64(dtStr);
            dtLong *= 10000;
            dtLong += 621355968000000000;
            var dt = new DateTime(dtLong).ToLocalTime();
            return dt;
        }
    }
}
