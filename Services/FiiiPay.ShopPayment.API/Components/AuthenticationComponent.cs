using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.ShopPayment.API.Components
{
    public class AuthenticationComponent
    {
        public UserAccount TokenValidate(string authHeader)
        {
            var accessToken = AccessTokenGenerator.DecryptToken(authHeader);
            var cacheKeyPrefix = "FiiiShop:Token:";
            var cacheKey = $"{cacheKeyPrefix}{accessToken.Identity}";
            var cacheToken = RedisHelper.StringGet(Constant.REDIS_TOKEN_DBINDEX, cacheKey);

            if (string.IsNullOrEmpty(cacheToken))
                throw new AccessTokenExpireException();

            if (authHeader != cacheToken)
            {
                throw new UnauthorizedException();
            }

            var id = Guid.Parse(accessToken.Identity);

            var account = new UserAccountDAC().GetById(id);

            if (account == null)
                throw new UnauthorizedException();
            if (account.Status == 0)
            {
                //已经禁用的用户，删除token
                RedisHelper.KeyDelete(Constant.REDIS_TOKEN_DBINDEX, cacheKey);
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, "Invalid user");
            }
            RedisHelper.StringSet(Constant.REDIS_TOKEN_DBINDEX, cacheKey, cacheToken,
                TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));
            return account;
        }
    }
}