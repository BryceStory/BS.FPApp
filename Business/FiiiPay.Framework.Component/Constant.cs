using System.Configuration;

namespace FiiiPay.Framework.Component
{
    public static class Constant
    {
        public static readonly string BLOB_BASE_URL = ConfigurationManager.AppSettings.Get("BlobBaseUrl");
        public static readonly string BLOB_KEY = ConfigurationManager.AppSettings.Get("BlobKey");

        public static readonly string SEND_SMS_LIMIT = ConfigurationManager.AppSettings.Get("Send_Sms_Limit");

        public static readonly string JPUSH_TAG = ConfigurationManager.AppSettings.Get("JpushTag");

        public static readonly string FP_EMAIL_API__URL = ConfigurationManager.AppSettings.Get("FP_EMAIL_API__URL");
        public static readonly string EMAIL_TOKEN = ConfigurationManager.AppSettings.Get("Email_Token");

        //验证信息缓存时间
        public const int VERIFICATION_CACHE_TIME = 5;//minutes
        //短信验证码缓存时间
        public const int SMS_CACHE_TIME = 30;//minutes
        //短信验证码有效期
        public const int SMS_EXPIRED_TIME = 5;//minutes
        //短信验证码发送时间间隔
        public const int SMS_SEND_INTERVAL = 1;
        //邮箱验证码有效期
        public const int EMAIL_EXPIRED_TIME = 20;//minutes
        //临时令牌有效期,minutes
        public const int TEMPTOKEN_EXPIRED_TIME = 30;
        //验证失败次数限制
        public const int VIRIFY_FAILD_TIMES_LIMIT = 5;
        //验证失败锁定时间，minutes
        public const int VIRIFY_FAILD_LOCK_TIME = 60;
        //注册失败多次锁定时间
        public const int REGISTER_FAILD_LOCK_TIME = 10;

        //token redis服务器数据库序号
        public const int REDIS_TOKEN_DBINDEX = 1;
        //短信、验证信息 redis服务器数据库序号
        public const int REDIS_SMS_DBINDEX = 2;
        //语言 redis服务器数据库序号
        public const int REDIS_LANGUAGE_DBINDEX = 3;

        //加密币汇率缓存时间（分）
        public const int CACHE_CRYPTO_PRICE_TIME = 5;

        //支付码 redis db index
        public const int REDIS_PAYMENT_CODE_DBINDEX = 4;
        //支付码 redis 前缀
        public const string REDIS_PAYMENT_CODE_PREFIX = "FiiiPay:PaymentCode:";
        //支付码过期时间
        public const int PAYMENT_CODE_EXPIRE_MINUTE = 3;
        //支付码前缀
        public const string PAYMENT_CODE_PREFIX = "62";

        /// <summary>
        /// 证件号使用次数限制
        /// </summary>
        public const int IDENTITY_LIMIT = 7;
    }
}
