using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework
{
    /// <summary>
    /// SHA256
    /// </summary>
    public class HMACSHA256
    {
        public static byte[] Hash(byte[] bytes)
        {
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(bytes);
        }

        public static byte[] DoubleHash(byte[] bytes)
        {
            return Hash(
                    Hash(bytes)
                );
        }

        public static byte[] EmptyHash()
        {
            return new byte[32];
        }
    }
}
