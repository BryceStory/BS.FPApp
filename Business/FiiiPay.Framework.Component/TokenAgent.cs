using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using log4net;

namespace FiiiPay.Data.Agents
{
    public class TokenAgent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TokenAgent));

        //private static readonly string _baseUrl = ConfigurationManager.AppSettings["FP_TOKEN_API__URL"];
        //private static readonly string _getMerchantCodeUrl = _baseUrl + "/Merchant/GetTokenInfo?merchantCode={0}&delete={1}";
        //private static readonly string _getMerchantCodeListUrl = _baseUrl + "/Merchant/GetTokenInfoList?merchantCodes={0}&delete={1}";
        //private static readonly string _getUserCodeUrl = _baseUrl + "/UserCurrencyAccount/GetTokenInfo?paymentCode={0}&delete={1}";
        //private static readonly string _getUserCodeListUrl = _baseUrl + "/UserCurrencyAccount/GetTokenInfoList?paymentCodes={0}&delete={1}";
        //private static readonly string _failedResult = "Unrecognize token.";

        private static readonly int _defaultUserTokenDbIndex = 10;
        private static readonly int _defaultMerchantTokenDbIndex = 11;

        private const string FiiiPayKey = "FiiiPay:OTP:{0}";
        private const string FiiiPOSKey = "FiiiPOS:OTP:{0}";

        /// <summary>
        /// 取出这个随机码对应的商家信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="delete">是否在取出后删除</param>
        /// <returns></returns>
        public MerchantTokenInfo GetMerchantIntoByCode(string code, bool delete = true)
        {
            //var str = RestUtilities.GetJson(string.Format(_getMerchantCodeUrl, code, delete ? 1 : 0));
            //if (str == _failedResult)
            //{
            //    return null;
            //}
            //return JsonConvert.DeserializeObject<MerchantTokenInfo>(str);

            var dbIndex = _defaultMerchantTokenDbIndex;
            var db = ConfigurationManager.AppSettings.Get("Merchant_Token_Db");
            if (!string.IsNullOrWhiteSpace(db))
                dbIndex = Convert.ToInt32(db);

            var key = string.Format(FiiiPOSKey, code);
            var c = RedisHelper.StringGet(dbIndex, key);
            if (string.IsNullOrWhiteSpace(c))
            {
                _log.InfoFormat("Not found code {0}", key);
                return null;
            }
            if (delete) 
            {
                RedisHelper.KeyDelete(dbIndex, string.Format(FiiiPOSKey, code));
            }

            return new MerchantTokenInfo
            {
                MerchantId = Guid.Parse(c),
                MerchantCode = code
            };
        }

        /// <summary>
        /// 取出这些随机码对应的商家信息
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="delete">是否在取出后删除</param>
        /// <returns></returns>
        public List<MerchantTokenInfo> GetMerchantIntoListByCodes(string codes, bool delete = true)
        {
            var cs = codes.Split(',');
            return cs.Select(s => GetMerchantIntoByCode(s, delete)).Where(t => t != null).ToList();

            //var str = RestUtilities.GetJson(string.Format(_getMerchantCodeListUrl, codes, delete ? 1 : 0));
            //return JsonConvert.DeserializeObject<List<MerchantTokenInfo>>(str);
        }

        /// <summary>
        /// 取出这个随机码对应的用户信息
        /// </summary>
        /// <param name="code">使用半角逗号,拼接</param>
        /// <param name="delete">是否在取出后删除</param>
        /// <returns></returns>
        public UserCurrencyAccountTokenInfo GetUserByCode(string code, bool delete = true)
        {
            var dbIndex = _defaultUserTokenDbIndex;
            var db = ConfigurationManager.AppSettings.Get("User_Token_Db");
            if (!string.IsNullOrWhiteSpace(db))
                dbIndex = Convert.ToInt32(db);

            var c = RedisHelper.StringGet(dbIndex, string.Format(FiiiPayKey, code));
            if (string.IsNullOrWhiteSpace(c)) return null;
            if (delete)
            {
                RedisHelper.KeyDelete(dbIndex, string.Format(FiiiPayKey, code));
            }

            return new UserCurrencyAccountTokenInfo
            {
                AccountId = Guid.Parse(c),
                PaymentCode = code
            };
            //var str = RestUtilities.GetJson(string.Format(_getUserCodeUrl, code, delete ? 1 : 0));

            //if (str.Contains(_failedResult))
            //{
            //    return null;
            //}

            //return JsonConvert.DeserializeObject<UserCurrencyAccountTokenInfo>(str);
        }

        ///// <summary>
        ///// 取出这些随机码对应的用户信息
        ///// </summary>
        ///// <param name="codes">使用半角逗号,拼接</param>
        ///// <param name="delete">是否在取出后删除</param>
        ///// <returns></returns>
        //public List<UserCurrencyAccountTokenInfo> GetUserListByCodes(string codes, bool delete = true)
        //{
        //    var str = RestUtilities.GetJson(string.Format(_getUserCodeListUrl, codes, delete ? 1 : 0));
        //    return JsonConvert.DeserializeObject<List<UserCurrencyAccountTokenInfo>>(str);
        //}
    }
}
