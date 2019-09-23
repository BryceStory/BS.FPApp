using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FiiiPay.Framework.Component.Authenticator
{
    /// <summary>
    /// 谷歌身份验证
    /// </summary>
    public class GoogleAuthenticator
    {
        public static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 前后偏移时间，分钟
        /// </summary>
        public int TimeOffset = 1;

        public string Issuser = "FiiiPay";

        public TimeSpan DefaultClockDriftTolerance { get; set; }

        public bool UseManagedSha1Algorithm { get; set; }

        public bool TryUnmanagedAlgorithmOnFailure { get; set; }

        public GoogleAuthenticator() : this(true, true) { }

        public GoogleAuthenticator(bool useManagedSha1, bool useUnmanagedOnFail)
        {
            DefaultClockDriftTolerance = TimeSpan.FromMinutes(TimeOffset);
            UseManagedSha1Algorithm = useManagedSha1;
            TryUnmanagedAlgorithmOnFailure = useUnmanagedOnFail;
        }

        public string GenerateSecretKey()
        {
            //var code = RandomAlphaNumericGenerator.GenerateCode(10);
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
        }

        public SetupCode GenerateSetupCode(SystemPlatform platForm, string accountTitleNoSpaces)
        {
            return GenerateSetupCode(platForm.ToString(), accountTitleNoSpaces, GenerateSecretKey());
        }

        /// <summary>
        /// 生成用户可扫描的谷歌认证初始码
        /// </summary>
        /// <param name="platForm">issuser</param>
        /// <param name="accountTitleNoSpaces">帐户名</param>
        /// <param name="accountSecretKey">共享密钥</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(SystemPlatform platForm, string accountTitleNoSpaces, string accountSecretKey)
        {
            return GenerateSetupCode(platForm.ToString(), accountTitleNoSpaces, accountSecretKey);
        }

        /// <summary>
        /// 生成用户可扫描的谷歌认证初始码
        /// </summary>
        /// <param name="issuer">发行者</param>
        /// <param name="accountTitleNoSpaces">帐户名</param>
        /// <param name="accountSecretKey">共享密钥</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(string issuer, string accountTitleNoSpaces, string accountSecretKey)
        {
            if (string.IsNullOrEmpty(accountTitleNoSpaces)) { throw new NullReferenceException("Account Title is null"); }

            accountTitleNoSpaces = accountTitleNoSpaces.Replace(" ", "");

            SetupCode sC = new SetupCode();
            sC.Account = accountTitleNoSpaces;
            sC.AccountSecretKey = accountSecretKey;

            //base32加密
            string encodedSecretKey = Base32Encode.Encode(accountSecretKey);
            sC.ManualEntryKey = encodedSecretKey;

            string provisionUrl = null;

            if (string.IsNullOrEmpty(issuer))
            {
                provisionUrl = String.Format("otpauth://totp/{0}?secret={1}", accountTitleNoSpaces, encodedSecretKey);
            }
            else
            {
                provisionUrl = String.Format("otpauth://totp/{0}?secret={1}&issuer={2}", accountTitleNoSpaces, encodedSecretKey, UrlEncode(issuer));
            }

            sC.QrCodeSetupUrl = provisionUrl;

            return sC;
        }

        private string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            foreach (char symbol in value)
            {
                if (validChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString().Replace(" ", "%20");
        }
        
        public string GeneratePINAtInterval(string accountSecretKey, long counter, int digits = 6)
        {
            return GenerateHashedCode(accountSecretKey, counter, digits);
        }

        internal string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
        {
            byte[] key = Encoding.UTF8.GetBytes(secret);
            return GenerateHashedCode(key, iterationNumber, digits);
        }

        internal string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            HMACSHA1 hmac = getHMACSha1Algorithm(key);

            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[hash.Length - 1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            int binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | (hash[offset + 3]);

            int password = binary % (int)Math.Pow(10, digits);
            return password.ToString(new string('0', digits));
        }

        private long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, _epoch, 30);
        }

        private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        /// <summary>
        /// Creates a HMACSHA1 algorithm to use to hash the counter bytes. By default, this will attempt to use
        /// the managed SHA1 class (SHA1Manager) and on exception (FIPS-compliant machine policy, etc) will attempt
        /// to use the unmanaged SHA1 class (SHA1CryptoServiceProvider).
        /// </summary>
        /// <param name="key">User's secret key, in bytes</param>
        /// <returns>HMACSHA1 cryptographic algorithm</returns>        
        private HMACSHA1 getHMACSha1Algorithm(byte[] key)
        {
            HMACSHA1 hmac;

            try
            {
                hmac = new HMACSHA1(key, UseManagedSha1Algorithm);
            }
            catch (InvalidOperationException ioe)
            {
                if (UseManagedSha1Algorithm && TryUnmanagedAlgorithmOnFailure)
                {
                    try
                    {
                        hmac = new HMACSHA1(key, false);
                    }
                    catch (InvalidOperationException ioe2)
                    {
                        throw ioe2;
                    }
                }
                else
                {
                    throw ioe;
                }
            }

            return hmac;
        }

        public bool ValidatePIN(string accountSecretKey, string pinCode)
        {
            return ValidatePIN(accountSecretKey, pinCode, DefaultClockDriftTolerance);
        }

        public bool ValidatePIN(string accountSecretKey, string pinCode, TimeSpan timeTolerance)
        {
            accountSecretKey = Base32Encode.Decode(accountSecretKey);
            var codes = GetCurrentPINs(accountSecretKey, timeTolerance);
            return codes.Any(c => c == pinCode);
        }

        public string GetCurrentPIN(string accountSecretKey)
        {
            return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter());
        }

        public string GetCurrentPIN(string accountSecretKey, DateTime now)
        {
            return GeneratePINAtInterval(accountSecretKey, GetCurrentCounter(now, _epoch, 30));
        }

        public string[] GetCurrentPINs(string accountSecretKey)
        {
            return GetCurrentPINs(accountSecretKey, DefaultClockDriftTolerance);
        }

        public string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance)
        {
            List<string> codes = new List<string>();
            long iterationCounter = GetCurrentCounter();
            int iterationOffset = 0;

            if (timeTolerance.TotalSeconds > 30)
            {
                iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00);
            }

            long iterationStart = iterationCounter - iterationOffset;
            long iterationEnd = iterationCounter + iterationOffset;

            for (long counter = iterationStart; counter <= iterationEnd; counter++)
            {
                codes.Add(GeneratePINAtInterval(accountSecretKey, counter));
            }

            return codes.ToArray();
        }
    }
    /// <summary>
    /// 身份信息
    /// </summary>
    public class SetupCode
    {
        /// <summary>
        /// 帐户名
        /// </summary>
        public string Account { get; internal set; }

        /// <summary>
        /// 共享密钥
        /// </summary>
        public string AccountSecretKey { get; internal set; }

        /// <summary>
        /// 手动输入密钥
        /// </summary>
        public string ManualEntryKey { get; internal set; }
        
        /// <summary>
        /// 二维码内容
        /// </summary>
        public string QrCodeSetupUrl { get; internal set; }
    }
}
