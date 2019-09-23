using System;
using System.Linq;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// 二维码操作类
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// 生成有规则的二维码字符串
        /// </summary>
        /// <param name="plateform">系统</param>
        /// <param name="business">业务</param>
        /// <returns>二维码字符串</returns>
        public static string Generate(int plateform, int business)
        {
            if (!Enum.IsDefined(typeof(SystemPlatform), plateform) || !Enum.IsDefined(typeof(QRCodeEnum), business))
                throw new ArgumentException();

            string platformStr = plateform.ToString();
            string actionStr = business.ToString();

            return $"{platformStr}-{actionStr}-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// 生成有规则的二维码字符串
        /// </summary>
        /// <param name="plateform">系统</param>
        /// <param name="business">业务</param>
        /// <param name="code">信息数据</param>
        /// <returns>二维码字符串</returns>
        public static string Generate(int plateform, int business, string code)
        {
            if (!Enum.IsDefined(typeof(SystemPlatform), plateform) || !Enum.IsDefined(typeof(QRCodeEnum), business))
                throw new ArgumentException();

            string platformStr = plateform.ToString();
            string actionStr = business.ToString();

            return $"{platformStr}-{actionStr}-{code}";
        }

        /// <summary>
        /// 生成有规则的二维码字符串
        /// </summary>
        /// <param name="plateform">系统</param>
        /// <param name="business">业务</param>
        /// <param name="code">信息数据</param>
        /// <returns>二维码字符串</returns>
        public static string Generate(SystemPlatform plateform, QRCodeEnum business, string code)
        {
            if (!Enum.IsDefined(typeof(SystemPlatform), plateform) || !Enum.IsDefined(typeof(QRCodeEnum), business))
                throw new ArgumentException();

            string platformStr = ((int)plateform).ToString();
            string actionStr = ((int)business).ToString();

            return $"{platformStr}-{actionStr}-{code}";
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="code">二维码字符串</param>
        /// <returns>二维码实体</returns>
        public static QRCodeEntity Deserialize(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new CommonException(ReasonCode.INVALID_QRCODE, GeneralResources.InvalidQRCode);
            var keys = code.Split(new[] { '-' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (keys.Length < 3)
                throw new CommonException(ReasonCode.INVALID_QRCODE, GeneralResources.InvalidQRCode);

            var codeValidate = true;
            int nPlatform, nBusiness = 0;
            codeValidate = int.TryParse(keys[0], out nPlatform);
            codeValidate = codeValidate && int.TryParse(keys[1], out nBusiness);
            if(!codeValidate)
                throw new CommonException(ReasonCode.INVALID_QRCODE, GeneralResources.InvalidQRCode);
            
            codeValidate = codeValidate && Enum.IsDefined(typeof(SystemPlatform), nPlatform);
            codeValidate = codeValidate && Enum.IsDefined(typeof(QRCodeEnum), nBusiness);
            if (!codeValidate)
                throw new CommonException(ReasonCode.INVALID_QRCODE, GeneralResources.InvalidQRCode);

            return new QRCodeEntity
            {
                SystemPlatform = (SystemPlatform)nPlatform,
                QrCodeEnum = (QRCodeEnum)nBusiness,
                QRCodeKey = keys[2]
            };
        }
    }

    public class QRCodeEntity
    {
        public SystemPlatform SystemPlatform { get; set; }
        public QRCodeEnum QrCodeEnum { get; set; }
        public string QRCodeKey { get; set; }
    }
}
