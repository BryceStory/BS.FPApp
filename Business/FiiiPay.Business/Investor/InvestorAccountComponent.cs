using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;
using FiiiPay.DTO.Investor;
using System;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;

namespace FiiiPay.Business
{
    public class InvestorAccountComponent
    {
        public InvestorAccount GetByUsername(string username)
        {
            var user = new InvestorAccountDAC().GetByUsername(username);
            return user;
        }

        public SignonDTO Login(string username, string password)
        {
            InvestorAccount user = CheckUser(username, password);

            return IssueAccessToken(user);
        }

        private InvestorAccount CheckUser(string username, string password)
        {
            var user = new InvestorAccountDAC().GetByUsername(username);
            if (user == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.AccountNotExist);
            }
            var securityVerify = new SecurityVerification(SystemPlatform.FiiiCoinWork);
            var loginErrorCountsInt = securityVerify.CheckErrorCount(SecurityMethod.Password, user.Id.ToString());
            if (user.Status == 0)
            {
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, R.该账户已被禁用);
            }
            if (!PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                securityVerify.IncreaseErrorCount(SecurityMethod.Password, user.Id.ToString());
            }
            securityVerify.DeleteErrorCount(SecurityMethod.Password, user.Id.ToString());
            return user;
        }

        private SignonDTO IssueAccessToken(InvestorAccount user)
        {
            var accessToken = AccessTokenGenerator.IssueToken(user.Username);

            var keyLoginToken = $"{SystemPlatform.FiiiCoinWork}:Investor:{user.Username}";
            RedisHelper.StringSet(keyLoginToken, accessToken, TimeSpan.FromSeconds(AccessTokenGenerator.EXPIRY_TIME));

            return new SignonDTO
            {
                AccessToken = accessToken
            };
        }

        public AccountInfoDTO GetUserInfo(InvestorAccount investor)
        {
            return new AccountInfoDTO
            {
                Username = investor.Username,
                InvestorName = investor.InvestorName,
                Cellphone = GetMaskedCellphone("", investor.Cellphone),
                IsResetPassword = investor.IsUpdatePassword,
                IsResetPIN = investor.IsUpdatePin
            };
        }

        public string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }

        public void ModifyPassword(InvestorAccount investor, string oldPassword, string newPassword)
        {
            string plainNewPassword = AES128.Decrypt(newPassword, AES128.DefaultKey);
            string plainOldPassword = AES128.Decrypt(oldPassword, AES128.DefaultKey);

            if (!PasswordHasher.VerifyHashedPassword(investor.Password, plainOldPassword))
            {
                throw new CommonException(ReasonCode.WRONG_OLD_PASSWORD_ENTERRED, R.原密码不正确);
            }
            if (PasswordHasher.VerifyHashedPassword(investor.Password, plainNewPassword))
            {
                throw new CommonException(ReasonCode.WRONG_OLD_PASSWORD_ENTERRED, R.新旧密码不能一致);
            }
            new InvestorAccountDAC().UpdatePassword(investor.Id, PasswordHasher.HashPassword(plainNewPassword));
            new InvestorAccountDAC().UpdatePasswordStatus(investor.Id, 1);
        }

        public void ModifyPIN(InvestorAccount investor, string oldPIN, string newPIN)
        {
            string plainNewPIN = AES128.Decrypt(newPIN, AES128.DefaultKey);
            if (PasswordHasher.VerifyHashedPassword(investor.PIN, plainNewPIN))
            {
                throw new CommonException(ReasonCode.PIN_MUST_BE_DIFFERENT, R.新旧Pin码不能一致);
            }
            VerifyPIN(investor, oldPIN);
            new InvestorAccountDAC().UpdatePIN(investor.Id, PasswordHasher.HashPassword(plainNewPIN));
            new InvestorAccountDAC().UpdatePINStatus(investor.Id, 1);
        }

        public string VerifyPIN(InvestorAccount investor, string encryptPin)
        {
            new SecurityComponent().InvestorVerifyPin(investor, encryptPin);
            return new SecurityVerification(SystemPlatform.FiiiCoinWork).GenegeToken(SecurityMethod.Pin);
        }
    }
}