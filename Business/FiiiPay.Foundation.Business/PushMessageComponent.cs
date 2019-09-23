using System;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;

namespace FiiiPay.Foundation.Business
{
    public class PushMessageComponent
    {
        public bool Auth(Guid clientId,string username,string password)
        {
            var accessModel = AccessTokenGenerator.DecryptToken(password);

            switch (accessModel.Platform)
            {
                case SystemPlatform.FiiiPay:
                    return Guid.TryParse(accessModel.Identity, out var userId) && clientId == userId;
                case SystemPlatform.FiiiPOS:
                    return accessModel.Identity == username;
                default:
                    return false;
            }
        }

        public bool AccessControlList(Guid clientId,string sysPlatform, string uId)
        {
            if (!Enum.TryParse(sysPlatform, out SystemPlatform platform))
                return false;

            return Guid.TryParse(uId, out var userId) && clientId == userId;
        }
    }
}
