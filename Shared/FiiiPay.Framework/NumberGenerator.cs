using System;
using System.Threading;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.NumberGenerator
    /// </summary>
    public class NumberGenerator
    {
        private static int _staticIncrement = new Random().Next();
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// 时间戳格式订单号
        /// </summary>
        /// <returns></returns>
        public static string GenerateUnixOrderNo()
        {
            var unixTimestamp = GetTimestampFromDateTime(DateTime.UtcNow);
            var incrementString = GetIncrementString();
            return $"{unixTimestamp}{incrementString}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetIncrementString()
        {
            int increment = Interlocked.Increment(ref _staticIncrement) & 65535;
            return new string(new[]
            {
                ToHexChar(increment >> 12 & 15),
                ToHexChar(increment >> 8 & 15),
                ToHexChar(increment >> 4 & 15),
                ToHexChar(increment & 15)
            });
        }

        private static char ToHexChar(int value)
        {
            return (char)(value + (value < 10 ? 48 : 87));
        }

        private static int GetTimestampFromDateTime(DateTime timestamp)
        {
            long num = (long)Math.Floor((ToUniversalTime(timestamp) - _unixEpoch).TotalSeconds);
            if (num < int.MinValue || num > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timestamp));
            return (int)num;
        }

        private static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            if (dateTime == DateTime.MaxValue)
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }
    }
}