using System;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;

namespace FiiiPOS.Web.Business.Helper
{
    /// <summary>
    /// 主要是Merchant_Web的读取redis的封装
    /// </summary>
    public class WebRedis
    {
        /// <summary>
        /// 设置登录码
        /// </summary>
        /// <param name="qrcode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetLoginQRCodeIndRedis(string qrcode,string value)
        {
            if (string.IsNullOrEmpty(qrcode))
                return false;

            string strKey = WebConfig.Redis_Key_QRCode + qrcode;
            return RedisHelper.StringSet(WebConfig.RedisDB_Web, strKey, value, new TimeSpan(0, 0, WebConfig.Redis_QRCode_ExpireTime));

        }

        /// <summary>
        /// 获取登录的app 商家的UserName
        /// </summary>
        /// <param name="qrcode"></param>
        /// <returns></returns>
        public static string GetLoginQRCodeIndRedis(string qrcode)
        {
            if (string.IsNullOrEmpty(qrcode))
                return "";

            string strKey = WebConfig.Redis_Key_QRCode + qrcode;
            return RedisHelper.StringGet(WebConfig.RedisDB_Web, strKey);
        }

        /// <summary>
        /// 获取到后，要销毁这个key
        /// </summary>
        /// <param name="qrcode"></param>
        /// <returns></returns>
        public static bool DeleteLoginQRCodeIndRedis(string qrcode)
        {
            if (string.IsNullOrEmpty(qrcode))
                return false;

            string strKey = WebConfig.Redis_Key_QRCode + qrcode;
            return RedisHelper.KeyDelete(WebConfig.RedisDB_Web, strKey);
        }


        /// <summary>
        /// 生成Web token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static bool SetWebTokenIndRedis(string merchantId, out string accessToken)
        {
            accessToken = null;
            if (string.IsNullOrEmpty(merchantId))
                return false;

            string key = WebConfig.Redis_Key_Token + merchantId.ToString();
            accessToken = AccessTokenGenerator.IssueToken(merchantId);

            bool result = RedisHelper.StringSet(WebConfig.RedisDB_Web, key, accessToken, new TimeSpan(0, 0, WebConfig.RedisDB_Token_ExpireTime));
  
            return result;

        }

        /// <summary>
        /// 获取web token 里的userName
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool GetWebTokenIndRedis(string token,out string userName)
        {
            userName = string.Empty;
            AccessToken accessToken = AccessTokenGenerator.DecryptToken(token);
            if (accessToken == null)
                return false;

            string key = WebConfig.Redis_Key_Token + accessToken.Identity;

            string redisToken = RedisHelper.StringGet(WebConfig.RedisDB_Web, key);
            if (redisToken != token)
                return false;

            userName = accessToken.Identity;
            return true;
        }

    }
}
