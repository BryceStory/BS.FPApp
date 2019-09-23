using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace FiiiPay.Business
{
    public class GatewayAgent
    {
        ///// <summary>
        ///// 请求接口
        ///// </summary>
        ///// <param name="apiName">接口名称</param>
        ///// <param name="dicParams">接口参数</param>
        ///// <returns></returns>
        //public static string PostToIndentity(string apiName, Dictionary<string, string> dicParams)
        //{
        //    string appId = ConfigurationManager.AppSettings["FiiiIndentityClientKey"];
        //    string secretKey = ConfigurationManager.AppSettings["FiiiIndentitySecretKey"];
        //    string baseUrl = ConfigurationManager.AppSettings["FiiiIndentityUrl"];
        //    int expired = 600;
        //    dicParams.Add("appid", appId);
        //    string strParams = JsonConvert.SerializeObject(dicParams);
        //    string strHttpUrl = $"{baseUrl.TrimEnd('/')}/{apiName.TrimStart('/')}";
        //    Dictionary<string, string> header = new Dictionary<string, string>();
        //    var t = ConvertDateTimeToInt(DateTime.Now);
        //    string sign = GetAppSign(appId, apiName, t, expired, secretKey);
        //    header.Add("signature", sign);
        //    string result = RestUtilities.PostJson(strHttpUrl,header,strParams);
        //    return result;
        //}

        public static string GetToGateway(string apiName)
        {
            string baseUrl = ConfigurationManager.AppSettings["Gateway_URL"];
            string clientKey = ConfigurationManager.AppSettings["GatewayClientKey"];
            string secretKey = ConfigurationManager.AppSettings["GatewaySecretKey"];
            
            string strHttpUrl = $"{baseUrl.TrimEnd('/')}/{apiName.TrimStart('/')}";

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Authorization", "bearer " + GenerateToken(clientKey, secretKey));

            string result = RestUtilities.GetJson(strHttpUrl, header);
            return result;
        }

        ///// <summary>
        ///// 获取签名
        ///// </summary>
        ///// <param name="apiName"></param>
        ///// <returns>//eT2jDFyBEwbDqtldumuAHJghwiZhPTQ2ODkmbT1nZXR1c2VyJnQ9MTUyNzQ5NTEzMyZlPTYwMA==   y=�\�ê�]�k��!�&a=4689&m=getuser&t=1527495133&e=600</returns>
        //private static string GetAppSign(string appId, string apiName, long timestamp, int expired, string secretKey)
        //{
        //    //var t = DateTimeHelper.ConvertDateTimeToInt(DateTime.Now);
        //    string plainText = $"a={appId}&m={apiName}&t={timestamp}&e={expired}";
        //    string sign = HmacSha1Sign(secretKey, plainText);
        //    return sign;
        //}

        ///// <summary>
        ///// HMAC-SHA1加密算法
        ///// </summary>
        ///// <param name="secret">密钥</param>
        ///// <param name="strOrgData">源文</param>
        ///// <returns></returns>
        //private static string HmacSha1Sign(string secret, string strOrgData)
        //{
        //    var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
        //    var dataBuffer = Encoding.UTF8.GetBytes(strOrgData);
        //    var hashBytes = hmacsha1.ComputeHash(dataBuffer);
        //    var all = hashBytes.Concat(dataBuffer).ToArray();
        //    return Convert.ToBase64String(all);
        //}
        
        ///// <summary>    
        ///// 距离1970-01-01的秒数  
        ///// </summary>    
        ///// <param name="time"> DateTime时间格式</param>    
        ///// <returns>Unix时间戳格式</returns>    
        //public static long ConvertDateTimeToInt(DateTime time)
        //{
        //    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //    return (int)(time - startTime).TotalSeconds;
        //}

        private static string GenerateToken(string clientKey, string secretKey)
        {
            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + clientKey;
            string token = AES128.Encrypt(password, secretKey);

            return token;
        }
    }
}
