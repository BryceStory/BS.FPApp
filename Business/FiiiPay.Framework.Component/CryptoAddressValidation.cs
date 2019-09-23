using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// 币种地址校验
    /// </summary>
    public class CryptoAddressValidation
    {
        private static bool IsProduction
        {
            get
            {
                string isProductionConfig = ConfigurationManager.AppSettings["IsProduction"];
                return isProductionConfig == "1";
            }
        }
        /// <summary>
        /// 校验币种地址的有效性
        /// </summary>
        /// <param name="cryptoCode">币种Code</param>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public static void ValidateAddress(string cryptoCode,string address)
        {
            bool result;
            switch (cryptoCode.ToUpper())
            {
                case "FIII":
                    //if (IsProduction)
                    //    regstr = @"^fiiim\w{33}$";
                    //else
                    //    regstr = @"^fiiit\w{33}$";
                    //result = System.Text.RegularExpressions.Regex.IsMatch(address, regstr);
                    result = FIIIAddressVerfy(IsProduction, address);
                    break;
                default:
                    return;
            }
            LogManager.GetLogger(typeof(CryptoAddressValidation)).Info($"-------------------------{result}");
            if (!result)
                throw new CommonException(ReasonCode.INVALID_ADDRESS, GeneralResources.EMInvalidAddress);
            return;
        }

        /// <summary>
        /// 校验币种Tag的有效性
        /// </summary>
        /// <param name="cryptoCode">币种Code</param>
        /// <param name="tag">tag</param>
        /// <returns></returns>
        public static void ValidateTag(string cryptoCode, string tag)
        {
            string regstr;
            bool result;
            switch (cryptoCode.ToUpper())
            {
                case "XRP":
                    regstr = @"^(429496729[0-5]|42949671\d{2}|4294966\d{3}|429495\d{4}|42948\d{5}|4293\d{6}|428\d{7}|41\d{8}|[1-3]\d{9}|[1-9]\d{0,8}|0)$";
                    result = System.Text.RegularExpressions.Regex.IsMatch(tag, regstr);
                    break;
                default:
                    return;
            }
            if (!result)
                throw new CommonException(ReasonCode.INVALID_TAG, GeneralResources.EMInvalidTag);
            return;
        }

        public static bool FIIIAddressVerfy(bool isProduction, string address)
        {
            /*
                fiiim AwTkyjENBMK3Zb1C6SqLKzaQotEjiMY9e
                fiiit 2sTHKSFrYJHo1ddUNNHCKqJnAhjATgr6c

                Base58解码->buffer->拿出后面四个字节->对剩余部分做Hash计算并取出开头四个字节->比对
            */
            if (!string.IsNullOrEmpty(address))
            {
                if (address.Length == 38)
                {
                    if (isProduction)
                    {
                        if (address.StartsWith("fiiim"))
                        {
                            byte[] bytes = Base58.Decode(address);
                            byte[] temp = bytes.Skip(bytes.Length - 4).Take(4).ToArray();
                            byte[] calc = bytes.Take(bytes.Length - 4).ToArray();
                            byte[] diff = HMACSHA256.DoubleHash(calc).Take(4).ToArray();
                            return diff.SequenceEqual(temp);
                        }
                    }
                    else
                    {
                        if (address.StartsWith("fiiit"))
                        {
                            byte[] bytes = Base58.Decode(address);
                            byte[] temp = bytes.Skip(bytes.Length - 4).Take(4).ToArray();
                            byte[] calc = bytes.Take(bytes.Length - 4).ToArray();
                            byte[] diff = HMACSHA256.DoubleHash(calc).Take(4).ToArray();
                            return diff.SequenceEqual(temp);
                        }
                    }
                }
            }
            return false;
        }
    }
}
