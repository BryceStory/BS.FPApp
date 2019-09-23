using System;

namespace FiiiPay.Framework.Component
{
    public abstract class RedisCacheManger 
    {
        public abstract void SetToken(Guid id, string value);

        public abstract void SetToken(string id, string value);

        public abstract bool DeleteToken(string id);

        public abstract bool DeleteToken(Guid id);

        public abstract string GetToken(string id);

        public abstract void ExpireToken(string id);

        public abstract bool Expire(string key);

        public abstract string GetLanguage(Guid accountId);

        public abstract void SetNotificationId(Guid accountId, string value);

        public abstract bool DeleteNotificationId(Guid accountId);

        public abstract string GetNotificationId(Guid accountId);

        public abstract void ChangeLanguage(string id, string lang);

        public abstract void ChangeLanguage(Guid id, string lang);
    }
}
