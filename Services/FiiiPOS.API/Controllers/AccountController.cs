using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Filters;
using FiiiPOS.API.Models;
using FiiiPOS.Business;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        /// <summary>
        ///  根据SN判断是否已绑定帐号
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpGet, Route("HasBoundAccount")]
        public ServiceResult<bool> HasBoundAccount(string sn)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new MerchantAccountComponent();

            result.Data = cpt.HasBoundAccount(sn);

            return result;
        }

        /// <summary>
        ///  根据SN获取绑定的商户帐号
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpGet, Route("MerchantSimpleInfoBySN")]
        public ServiceResult<MechantSimpleInfoDTO> MerchantSimpleInfoBySn(string sn)
        {
            var result = new ServiceResult<MechantSimpleInfoDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var cpt = new MerchantAccountComponent();

            result.Data = cpt.GetMerchantSimpleInfoBySn(sn);

            return result;
        }


        /// <summary>
        /// 发送注册短信验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("SendSignupSMS")]
        public ServiceResult SendSignupSMS(SendSignupSMSModel model)
        {
            ServiceResult result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().SendSignupSMS(model.Cellphone, model.CountryId, model.POSSN);
            return result;
        }

        /// <summary>
        /// 验证注册短信验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [AllowAnonymous, HttpPost, Route("VerificationSMSCode")]
        public ServiceResult<string> VerificationSMSCode(VerificationSMSCodeModel model)
        {
            ServiceResult<string> result = new ServiceResult<string>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().VerificationSMSCode(model.CountryId, model.Cellphone, model.Code);
            return result;
        }

        /// <summary>
        /// 校验商家账号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("VerifyAccount")]
        public ServiceResult<bool> VerifyAccount(VerifyAccountModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().VerifyAccount(model.MerchantAccount, model.MerchantName, model.InvitationCode);
            result.Data = true;

            return result;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("Signup")]
        public ServiceResult<SignonDTO> Signup(SignupModel model)
        {
            ServiceResult<SignonDTO> result = new ServiceResult<SignonDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            if (!PinProcessor.TryParse(model.PIN, out string pin))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                result.Message = Resources.PIN格式不正确;
                return result;
            }


            result.Data = new MerchantAccountComponent().Signup(model.CountryId, model.Cellphone, model.MerchantAccount, model.MerchantName, model.POSSN, model.InvitationCode, pin);
            return result;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>access token</returns>
        [AllowAnonymous, HttpPost, Route("Signon")]
        public ServiceResult<SignonDTO> Signon(SignonModel model)
        {
            ServiceResult<SignonDTO> result = new ServiceResult<SignonDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            MerchantAccountComponent cpt = new MerchantAccountComponent();

            result.Data = cpt.Signon(model.POSSN, model.MerchantId, model.PIN);
            return result;
        }

        /// <summary>
        /// 绑定设备到推送服务
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("BindNoticeRegId")]
        public ServiceResult<bool> BindNoticeRegId(BindNoticeRegIdIM im)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            result.Data = new MerchantAccountComponent().BindNoticeRegId(this.GetMerchantAccountId(), im);
            return result;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SignOut")]
        public ServiceResult SignOut()
        {
            ServiceResult result = new ServiceResult();

            new MerchantAccountComponent().SignOut(this.GetMerchantAccountId());
            return result;
        }

        /// <summary>
        /// 商户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MerchantAccountInfo")]
        public ServiceResult<MerchantAccountDTO> MerchantAccountInfo()
        {
            var accountId = this.GetMerchantAccountId();

            var result = new ServiceResult<MerchantAccountDTO>
            {
                Data = new MerchantAccountComponent().GetMerchantInfoById(accountId)
            };


            return result;
        }

        ///// <summary>
        ///// 是否已设置PIN
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet, Route("HasSettingPIN")]
        //public ServiceResult<bool> HasSettingPIN()
        //{
        //    var result = new ServiceResult<bool>();

        //    var accountId = this.GetMerchantAccountId();
        //    var cpt = new MerchantAccountComponent();

        //    result.Data = cpt.HasSettingPIN(accountId);

        //    return result;
        //}

        ///// <summary>
        ///// 首次设置PIN
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost, Route("SettingPIN")]
        //public ServiceResult SettingPIN(SettingPINModel model)
        //{
        //    var result = new ServiceResult();

        //    if (!PinProcessor.TryParse(model.PIN, out string pin))
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        result.Message = Resources.PIN格式不正确;
        //        return result;
        //    }

        //    new MerchantAccountComponent().SettingPIN(this.GetMerchantAccountId(), pin);


        //    return result;
        //}

        /// <summary>
        /// 验证pin获得token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPINToken")]
        public ServiceResult<string> GetPINToken(GetPINTokenModel model)
        {
            var result = new ServiceResult<string>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            result.Data = new SecurityComponent().FiiiPOSVerfiyPinReturnToken(accountId, model.PIN);


            return result;
        }

        /// <summary>
        /// 修改PIN时，验证PIN码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyModifyPINPIN")]
        public ServiceResult VerifyModifyPINPIN(GetPINTokenModel model)
        {
            var result = new ServiceResult();
            new MerchantAccountComponent().VerifyModifyPINPIN(this.GetMerchantAccountId(), model.PIN);

            return result;
        }

        /// <summary>
        /// 修改PIN时，综合验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyModifyPINCombine")]
        public ServiceResult<string> VerifyModifyPINCombine(CombineVerifyModel model)
        {
            ServiceResult<string> result = new ServiceResult<string>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            new MerchantAccountComponent().VerifyModifyPINCombine(accountId, model.SMSCode, model.GoogleCode);

            return result;
        }

        /// <summary>
        /// 修改PIN
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ModifyPIN")]
        public ServiceResult ModifyPIN(ModifyPINModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            if (!PinProcessor.TryParse(model.PIN, out string pin))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                result.Message = Resources.PIN格式不正确;
                return result;
            }

            new MerchantAccountComponent().ModifyPIN(this.GetMerchantAccountId(), pin);

            return result;
        }

        ///// <summary>
        ///// 手机号修改pin
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost, Route("ModifyPINByCellphone")]
        //public ServiceResult ModifyPINByCellphone(ModifyPINByCellphoneModel model)
        //{
        //    var result = new ServiceResult();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }
        //    var pin = AES128.Decrypt(model.PIN, AES128.DefaultKey);

        //    new MerchantAccountComponent().ModifyPINByCellphone(this.GetMerchantAccountId(), model.CellphoneToken, pin);
        //    
        //    return result;
        //}

        ///// <summary>
        ///// 验证营业执照
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost, Route("VerifyBusinessLicense")]
        //public ServiceResult<bool> VerifyBusinessLicense(VerifyBusinessLicenseModel model)
        //{
        //    var result = new ServiceResult<bool>();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }

        //    result.Data = new MerchantAccountComponent().VerifyBusinessLicense(this.GetMerchantAccountId(), model.VerifySMSToken, model.BusinessLicense);
        //    
        //    return result;
        //}

        /// <summary>
        /// 商户支持收款的币种列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("SupportReceiptList")]
        public ServiceResult<List<MerchantSupportReceiptWalletDTO>> SupportReceiptList()
        {
            var result = new ServiceResult<List<MerchantSupportReceiptWalletDTO>>();
            var accountId = this.GetMerchantAccountId();
            var cpt = new MerchantWalletComponent();

            result.Data = cpt.SupportReceiptList(accountId);

            return result;
        }

        /// <summary>
        /// 根据法币获取收款的币种信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSupportReceiptByFiatCurrency")]
        public ServiceResult<MerchantSupportReceiptWalletDTO> GetSupportReceiptByFiatCurrency(string fiatCurrency, int coinId)
        {
            var result = new ServiceResult<MerchantSupportReceiptWalletDTO>();
            var accountId = this.GetMerchantAccountId();
            var cpt = new MerchantWalletComponent();

            result.Data = cpt.GetSupportReceiptByFiatCurrency(accountId, fiatCurrency, coinId);

            return result;
        }

        /// <summary>
        /// 反馈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Feedback")]
        public ServiceResult Feedback(FeedbackModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().Feedback(this.GetMerchantAccountId(), model.Content);

            return result;
        }

        /// <summary>
        /// FiiiPOS WEB端扫码登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanQRLogin")]
        public ServiceResult<bool> ScanQRLogin(ScanQRLoginInModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            result.Data = new MerchantAccountComponent().ScanQRLogin(this.GetMerchantAccountId(), model.QRCode);


            return result;
        }

        /// <summary>
        /// 修改商家名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyMerchantName")]
        public ServiceResult ModifyMerchantName(ModifyMerchantNameModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().ModifyMerchantName(this.GetMerchantAccountId(), model.MerchantName);


            return result;
        }

        /// <summary>
        /// 发送绑定短信验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("SendBindingSMS")]
        public ServiceResult SendBindingSMS(SendBindingSMSModel model)
        {
            ServiceResult result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            MerchantAccountComponent accountComponent = new MerchantAccountComponent();

            accountComponent.SendBindingSMS(model.Cellphone, model.CountryId, model.MerchantAccount, model.POSSN);



            return result;
        }


        /// <summary>
        /// 校验账号信息, 返回需要校验信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token</returns>
        [AllowAnonymous, HttpPost, Route("VerifyMerchantAccount")]
        public ServiceResult<AccountNeedVerifyInfo> VerifyMerchantAccount(VerifyMerchantAccountModel model)
        {
            var result = new ServiceResult<AccountNeedVerifyInfo>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            result.Data = new MerchantAccountComponent().VerifyMerchantAccount(model.CountryId, model.Cellphone, model.Code, model.MerchantAccount);
            return result;
        }

        /// <summary>
        /// 通过账号校验PIN
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token</returns>
        [AllowAnonymous, HttpPost, Route("VerifyPINByMerchantAccount")]
        public ServiceResult VerifyPINByMerchantAccount(VerifyPINByMerchantAccountModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().VerifyPINByMerchantAccount(model.EncryptPin, model.MerchantAccount);



            return result;
        }

        /// <summary>
        /// 通过账号校验google验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token</returns>
        [AllowAnonymous, HttpPost, Route("VerifyGoogleAuthByMerchantAccount")]
        public ServiceResult VerifyGoogleAuthByMerchantAccount(VerifyGoogleAuthByMerchantAccountModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().VerifyGoogleAuthByMerchantAccount(model.GoogleCode, model.MerchantAccount);



            return result;
        }

        /// <summary>
        /// 账号登录绑定pos机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("BindingAccount")]
        public ServiceResult<SignonDTO> BindingAccount(BindingAccountModel model)
        {
            var result = new ServiceResult<SignonDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            result.Data = new MerchantAccountComponent().BindingAccount(model.MerchantAccount, model.PosSN);
            return result;
        }

        /// <summary>
        /// 解绑账号,验证PIN
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUnbindAccountPin")]
        public ServiceResult<bool> VerifyUnbindAccountPin(GetPINTokenModel im)
        {
            new MerchantAccountComponent().VerifyUnBindAccountPin(this.GetMerchantAccountId(), im.PIN);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 解绑账号,综合验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUnbindAccountCombine")]
        public ServiceResult<bool> VerifyUnbindAccountCombine(BaseCombineVerifyModel im)
        {
            new MerchantAccountComponent().VerifyUnBindAccountCombine(this.GetMerchantAccountId(), im.SMSCode, im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 解绑账号
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("UnbindingAccount")]
        public ServiceResult UnbindingAccount()
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().UnbindingAccount(this.GetMerchantAccountId());
            return result;
        }

        /// <summary>
        /// 设置商家溢价率
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SettingMarkup")]
        public ServiceResult SettingMarkup(SettingMarkupModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            new MerchantAccountComponent().SettingMarkup(accountId, model.PerMarkup);


            return result;
        }

        /// <summary>
        /// 设置商家法币
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SettingFiatCurrency")]
        public ServiceResult SettingFiatCurrency(SettingFiatCurrencyModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new MerchantAccountComponent().SettingFiatCurrency(this.GetMerchantAccountId(), model.FiatCurrency);


            return result;
        }

        /// <summary>
        /// 账号校验
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckAccount"), AllowAnonymous]
        public ServiceResult<bool> CheckAccount(CheckAccountModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new MerchantAccountComponent().CheckAccount(model.MerchantId, model.SN);
            result.Data = true;


            return result;
        }

        /// <summary>
        /// 获取法币列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetFiatCurrency")]
        public ServiceResult<List<FiatCurrencyDTO>> FiatCurrency()
        {
            var currencyList = new CurrencyComponent().GetList();

            var result = new ServiceResult<List<FiatCurrencyDTO>>
            {
                Data = currencyList.Select(e => new FiatCurrencyDTO
                {
                    Id = e.ID,
                    Name = this.IsZH() ? e.Name_CN : e.Name,
                    Code = e.Code
                }).ToList()
            };

            return result;
        }

        /// <summary>
        /// 交易费底扣
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [HttpGet, Route(nameof(SetWithdrawalFeeType))]
        public ServiceResult SetWithdrawalFeeType(WithdrawalFeeType type)
        {
            var result = new ServiceResult();
            new MerchantAccountComponent().SetWithdrawalFeeType(this.GetMerchantAccountId(), type);

            return result;
        }


        /// <summary>
        /// Changes the language.
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("ChangeLanguage")]
        public ServiceResult ChangeLanguage(ChangeLanguageModel im)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            if (im.Language.Contains("en") || im.Language.Contains("zh"))
            {
                new MerchantAccountComponent().ChangeLanguagetoDb(this.GetMerchantAccountId(), im.Language);
            }

            result.Success();
            return result;
        }

        ///// <summary>
        ///// 设置挖矿地址
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost, Route("SettingMinerCryptoAddress")]
        //public ServiceResult SettingMinerCryptoAddress(SettingMinerCryptoAddressModel model)
        //{
        //    var result = new ServiceResult();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }

        //    new MerchantAccountComponent().SettingMinerCryptoAddress(this.GetMerchantAccountId(), model.Address);

        //    
        //    return result;
        //}


    }
}
