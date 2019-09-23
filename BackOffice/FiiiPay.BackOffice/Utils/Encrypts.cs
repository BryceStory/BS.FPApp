using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    public class Encrypts
    {
        private const string DefaultKey = "4148716E-ED5F-44FA-869E-E9D058D2DDC5";
        public static string GetEncryptString(string str)
        {
            return AES128.Encrypt(str + "_" + DateTime.UtcNow.ToUnixTime(), DefaultKey);
        }
        public static string GetDecryptString(string enStr)
        {
            return AES128.Decrypt(enStr, DefaultKey);
        }
        /// <summary>
        /// 产生10分钟精度的ticks
        /// </summary>
        /// <param name="minites">时间精度，精确到10分钟</param>
        /// <returns></returns>
        public static long GenerateTicksInTenTime()
        {
            double minuteTicks = 1000 * 60 * 10;
            var ticks = Math.Floor(DateTime.UtcNow.ToUnixTime() / minuteTicks);
            return (long)ticks;
        }
    }
}