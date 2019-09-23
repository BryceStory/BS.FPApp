using System.Configuration;

namespace FiiiPOS.Web.Business
{
    public static class WebConfig
    {
        /// <summary>
        /// Web商家登录二维码前缀
        /// </summary>
        public const string QRCodePrefix = "wm";

        /// <summary>
        /// redis连接字符串
        /// </summary>
        public static string RedisConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
        public const int RedisDB_Web = 1; //数据库
        public const string Redis_Key_QRCode = "FiiiPOS:Web:QRCode:";
        public const int Redis_QRCode_ExpireTime = 120;  //120秒

        public const string Redis_Key_Token = "FiiiPOS:Web:Token:";
        public const int RedisDB_Token_ExpireTime = 1209600;  //15 days
    }
}
