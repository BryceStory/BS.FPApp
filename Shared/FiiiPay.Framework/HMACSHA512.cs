using System;
using System.Text;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.HMACSHA512
    /// </summary>
    public static class HMACSHA512
    {
        /// <summary>
        /// Generate Identity Token for payment code
        /// </summary>
        /// <param name="secretKey">The secret key, must be 64 bytes (32 characters).</param>
        /// <param name="date">The date time of the token.</param>
        /// <returns></returns>
        public static string Generate(string secretKey, DateTime date)
        {
            date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var counter = (long)date.Subtract(unixTime).TotalSeconds;

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            ulong hash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes))
            {
                var counterBytes = BitConverter.GetBytes(counter);
                var hashBytes = hmac.ComputeHash(counterBytes);
                hash = BitConverter.ToUInt64(hashBytes, 0);
            }

            return hash.ToString();
        }

        /// <summary>
        /// Generate15s the specified secret key.
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string Generate15(string secretKey, DateTime date)
        {
            date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var counter = (long)date.Subtract(unixTime).TotalSeconds;

            byte[] keyBytes = Encoding.ASCII.GetBytes(secretKey);

            ulong hash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes))
            {
                byte[] counterBytes = BitConverter.GetBytes(counter);
                byte[] hashBytes = hmac.ComputeHash(counterBytes);
                hash = BitConverter.ToUInt64(hashBytes, 0);
            }

            ulong token = hash % (ulong)Math.Pow(10, 15);

            return token.ToString("000000000000000");
        }
    }
}
