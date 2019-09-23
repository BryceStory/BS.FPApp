using System.Security.Cryptography;
using System.Text;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.RandomAlphaNumericGenerator
    /// </summary>
    public static class RandomAlphaNumericGenerator
    {
        /// <summary>
        /// Generates the specified maximum size.
        /// </summary>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns></returns>
        public static string Generate(int maxSize)
        {
            var data = new byte[maxSize];

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns></returns>
        public static string GenerateCode(int maxSize)
        {
            byte[] data = new byte[maxSize];

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates all number.
        /// </summary>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns></returns>
        public static string GenerateAllNumber(int maxSize)
        {
            byte[] data = new byte[maxSize];

            var chars = "0123456789".ToCharArray();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}
