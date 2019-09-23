using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using System;

namespace FiiiPay.Business
{
    public class OpenAccountComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiiiType">0FiiiPay 1FiiiPos</param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public OpenAccount GetOpenAccount(FiiiType fiiiType, Guid accountId)
        {
            return new OpenAccountDAC().GetOpenAccount(fiiiType, accountId);
        }
        public OpenAccount Create(int platform, FiiiType fiiiType, Guid accountId)
        {
            OpenAccount account = new OpenAccount
            {
                CreateTime = DateTime.UtcNow,
                PlatformId = platform,
                FiiiType = fiiiType,
                OpenId = Guid.NewGuid(),
                SecretKey = RandomAlphaNumericGenerator.Generate(32),
                AccountId = accountId
            };

            long id = new OpenAccountDAC().Create(account);
            account.Id = id;

            return account;
        }
    }
}
