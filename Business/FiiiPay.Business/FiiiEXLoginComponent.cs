using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;
using Newtonsoft.Json;
using System;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business
{
    public class FiiiEXLoginComponent
    {
        public bool FiiiPayLogin(string QRCode, UserAccount account)
        {
            if(!RedisHelper.KeyExists(0,QRCode))
                throw new CommonException(ReasonCode.INVALID_QRCODE, R.无效二维码);

            var profile = new UserProfileAgent().GetUserProfile(account.Id);
            var country = new CountryDAC().GetById(account.CountryId);
            var openAccount = new OpenAccountDAC().GetOpenAccount(FiiiType.FiiiPay, account.Id);
            if (openAccount == null)
                openAccount = new OpenAccountComponent().Create((int)SystemPlatform.FiiiEX, FiiiType.FiiiPay, account.Id);

            var om = new
            {
                OpenId = openAccount.OpenId,
                AccountName = country.PhoneCode + " " + account.Cellphone,
                UserType = 0,//0FiiiPay 1FiiiPos
                CountryId = country.Id,
                PosCode = "",
                CountryName = country.Name,
                CountryName_CN = country.Name_CN,
                FullName = profile == null ? "" : (profile.FirstName + " " + profile.LastName),
                Cellphone = GetMaskedCellphone(country.PhoneCode, account.Cellphone)
            };
            RedisHelper.StringSet(0, QRCode, JsonConvert.SerializeObject(om), TimeSpan.FromMinutes(5));

            return true;
        }

        public bool FiiiPosLogin(string QRCode, Guid accountId)
        {
            if (!RedisHelper.KeyExists(0,QRCode))
                throw new CommonException(ReasonCode.INVALID_QRCODE, R.无效二维码);

            MerchantAccount account = new MerchantAccountDAC().GetById(accountId);
            if (account == null)
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.用户不存在);

            var country = new CountryDAC().GetById(account.CountryId);
            var pos = new POSDAC().GetById(account.POSId.Value);
            var openAccount = new OpenAccountDAC().GetOpenAccount(FiiiType.FiiiPOS, account.Id);
            if (openAccount == null)
                openAccount = new OpenAccountComponent().Create((int)SystemPlatform.FiiiEX, FiiiType.FiiiPOS, account.Id);

            var om = new
            {
                OpenId = openAccount.OpenId,
                AccountName = account.Username,
                UserType = 1,//0FiiiPay 1FiiiPos
                CountryId = country.Id,
                PosCode = pos.Sn,
                CountryName = country.Name,
                CountryName_CN = country.Name_CN,
                FullName = account.MerchantName,
                Cellphone = GetMaskedCellphone(account.PhoneCode, account.Cellphone)
            };
            RedisHelper.StringSet(0, QRCode, JsonConvert.SerializeObject(om), TimeSpan.FromMinutes(5));

            return true;
        }

        public bool HasExAccount(FiiiType fiiiType, Guid accountId)
        {
            var openAccount = new OpenAccountDAC().GetOpenAccount(fiiiType, accountId);
            return openAccount != null;
        }

        private string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }
    }
}
