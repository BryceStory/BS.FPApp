using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Verification;
using FiiiPay.ShopPayment.API.Models;
using System;
using System.Threading.Tasks;
using FiiiPay.Framework.Component.Verification;
using Newtonsoft.Json;

namespace FiiiPay.ShopPayment.API.Components
{
    internal class AccountComponent
    {
        public string GetLoginQRCode()
        {
            var token = Guid.NewGuid().ToString("N");
            var code = QRCode.Generate(SystemPlatform.FiiiShop, QRCodeEnum.FiiiPayLogin, token);
            return code;
        }

        public async Task<LoginDto> GetLoginStatus(string code)
        {
            var codeEntity = QRCode.Deserialize(code);

            if (codeEntity.SystemPlatform != SystemPlatform.FiiiShop || codeEntity.QrCodeEnum != QRCodeEnum.FiiiPayLogin)
                throw new CommonException(ReasonCode.INVALID_QRCODE, "");

            string cKey = "FiiiShop:Login:" + codeEntity.QRCodeKey;
            var loginInfo = RedisHelper.Get<LoginInfo>(Constant.REDIS_TOKEN_DBINDEX, cKey);
            if (loginInfo == null || loginInfo.Status == LoginStatus.UnLogined)
                throw new CommonException(ReasonCode.FiiiShopCode.ScanUnLogined, "");
            if (loginInfo.Status == LoginStatus.Logined)
            {
                var user = new UserAccountDAC().GetById(loginInfo.AccountId);
                RedisHelper.KeyDelete(Constant.REDIS_TOKEN_DBINDEX, cKey);
                return await IssueAccessToken(user);
            }
            throw new CommonException(ReasonCode.FiiiShopCode.ScanLogining, "");
        }

        /// <summary>
        /// 发送登录验证码
        /// </summary>
        /// <param name="phoneCode"></param>
        /// <param name="cellphone"></param>
        public bool SendLoginCode(string phoneCode, string cellphone)
        {
            var user = new UserAccountDAC().GetByFullPhoneCode(phoneCode, cellphone);
            if (user == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Properties.Resource.MsgAccountNotExist);
            }
            if (user.Status == 0)
            {
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, Properties.Resource.MsgAccountDisabled);
            }

            SecurityVerify.SendCode(new LoginCellphoneVerifier(), SystemPlatform.FiiiShop, $"{user.CountryId}:{cellphone}", $"{phoneCode}{cellphone}");

            return true;
        }

        public async Task<LoginDto> Login(LoginVo vo, string ip)
        {
            var user = new UserAccountDAC().GetByFullPhoneCode(vo.CountryPhoneCode, vo.Cellphone);
            SecurityVerify.Verify(new LoginCellphoneVerifier(), SystemPlatform.FiiiShop, $"{user.CountryId}:{vo.Cellphone}", vo.SmsCode);
            if (user == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Properties.Resource.MsgAccountNotExist);
            }
            if (user.Status == 0)
            {
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, Properties.Resource.MsgAccountDisabled);
            }
            return await IssueAccessToken(user);
        }

        private async Task<LoginDto> IssueAccessToken(UserAccount user)
        {
            //var keyLoginTokenPrefix = "FiiiShop:Token:";
            //var keyLoginToken = $"{keyLoginTokenPrefix}{user.Id}";
            //var accessToken = AccessTokenGenerator.IssueToken(user.Id.ToString());
            //RedisHelper.StringSet(Constant.REDIS_TOKEN_DBINDEX, keyLoginToken, accessToken, TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));

            ProfileRouter pRouter = new ProfileRouterDAC().GetRouter(user.CountryId);
            var profile = GetProfileByAccountId(pRouter, user.Id);

            string countryCode = "";
            if (profile.Country.HasValue)
            {
                var country = new CountryDAC().GetById(user.CountryId);
                if (country != null)
                {
                    countryCode = country.Code;
                }
            }

            var result = new LoginDto
            {
                UserId = user.Id.ToString("N"),
                Email=user.Email,
                LastName = profile.LastName,
                FirstName = profile.FirstName,
                CountryCode = countryCode,
                ProvinceName = profile.State,
                CityName = profile.City,
                Address = profile.Address1 + (string.IsNullOrEmpty(profile.Address1 + profile.Address2) ? "" : " ") + profile.Address2,
                Postcode = profile.Postcode,
                Cellphone = profile.Cellphone
            };
            return await Task.FromResult(result);
        }

        private UserProfile GetProfileByAccountId(ProfileRouter router, Guid id)
        {
            var url = $"{router.ServerAddress}/User/GetById";
            System.Collections.Generic.Dictionary<string, string> headers = new System.Collections.Generic.Dictionary<string, string>();
            var token = GenerateToken(router);
            headers.Add("Authorization", "Bearer " + token);
            var obj = new
            {
                Id = id
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<UserProfile>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        private string GenerateToken(ProfileRouter router)
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + router.ClientKey;
            var token = AES128.Encrypt(password, router.SecretKey);

            return token;
        }

        public bool CheckPin(Guid accountId, string pin)
        {
            var user = new UserAccountDAC().GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiShop, accountId.ToString(), user.Pin, pin);
            return true;
        }

        public CryptoWalletDto GetFiiiBalance(Guid accountId)
        {
            string coinCode = "FIII";
            var wallet = new UserWalletDAC().GetByCryptoCode(accountId, coinCode);
            return new CryptoWalletDto
            {
                Balance = (wallet == null ? 0 : wallet.Balance),
                FrozenBalance = (wallet == null ? 0 : wallet.FrozenBalance)
            };
        }
    }
}