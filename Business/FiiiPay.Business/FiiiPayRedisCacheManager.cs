using System;
using FiiiPay.Entities;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using StackExchange.Redis;

namespace FiiiPay.Business
{
    public class FiiiPayRedisCacheManager : RedisCacheManger
    {
        private const int REDIS_TOKEN_DBINDEX = 1;
        private const string Token = "Token";

        public override void SetToken(Guid id, string value)
        {
            SetToken(id.ToString(), value);
        }

        public override void SetToken(string id, string value)
        {
            var cacheKey = $"FiiiPay:Token:{id}";
            var database = GetTokenDatabase();
            database.HashSet(cacheKey, new[] { new HashEntry(Token, value) });
        }

        public override bool DeleteToken(string id)
        {
            var cacheKey = $"FiiiPay:Token:{id}";
            var database = GetTokenDatabase();

            return database.KeyDelete(cacheKey);
        }

        public override bool DeleteToken(Guid id)
        {
            return DeleteToken(id.ToString());
        }

        public override string GetToken(string id)
        {
            var cacheKey = $"FiiiPay:Token:{id}";
            var database = GetTokenDatabase();
            return database.HashGet(cacheKey, Token);
        }

        public override void ExpireToken(string id)
        {
            var cacheKey = $"FiiiPay:Token:{id}";
            var database = GetTokenDatabase();
            database.KeyExpire(cacheKey, TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));
        }
        
        public override bool Expire(string key)
        {
            var database = GetDefaultDatabase();
            return database.KeyExpire(key, TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));
        }

        public override bool DeleteNotificationId(Guid accountId)
        {
            var cacheKey = $"FiiiPay:Token:{accountId}";
            var database = GetTokenDatabase();

            return database.HashDelete(cacheKey, "NotificationId");
        }

        public override string GetNotificationId(Guid accountId)
        {
            var redisKey = $"FiiiPay:Token:{accountId}";
            var database = GetTokenDatabase();
            return database.HashGet(redisKey, "NotificationId");
        }

        public override void ChangeLanguage(string id, string lang)
        {
            var cacheKey = $"FiiiPay:Token:{id}";
            const string langKey = "Language";

            var database = GetTokenDatabase();
            database.HashSet(cacheKey, langKey, lang);
        }

        public override void ChangeLanguage(Guid id, string lang)
        {
            ChangeLanguage(id.ToString(), lang);
        }

        public override string GetLanguage(Guid accountId)
        {
            var cacheKey = $"FiiiPay:Token:{accountId}";
            const string langKey = "Language";
            var database = GetTokenDatabase();

            return database.HashGet(cacheKey, langKey);
        }

        public override void SetNotificationId(Guid accountId, string value)
        {
            var cacheKey = $"FiiiPay:Token:{accountId}";
            var database = GetTokenDatabase();

            database.HashSet(cacheKey, "NotificationId", value);
        }

        public UserAccount GetUserAccount(string accountId)
        {
            var account = new UserAccount();
            var cacheKey = $"FiiiPay:Token:{accountId}";
            var database = GetTokenDatabase();

            var status = database.HashGet(cacheKey, nameof(UserAccount.Status)).ToString();
            var countryId = database.HashGet(cacheKey, nameof(UserAccount.CountryId)).ToString();

            account.Status = string.IsNullOrWhiteSpace(status) ? byte.MinValue : byte.Parse(status);
            account.CountryId = string.IsNullOrWhiteSpace(countryId) ? 0 : int.Parse(countryId);
            account.FiatCurrency = database.HashGet(cacheKey, nameof(UserAccount.FiatCurrency));

            return account;
        }

        protected IDatabase GetTokenDatabase()
        {
            return RedisHelper.GetDatabase(REDIS_TOKEN_DBINDEX);
        }

        protected IDatabase GetDefaultDatabase()
        {
            return RedisHelper.GetDatabase(0);
        }
    }
}
