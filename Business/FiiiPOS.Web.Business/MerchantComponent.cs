using FiiiPay.Data;
using FiiiPay.Entities;
using System;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.Web.Business.Helper;

namespace FiiiPOS.Web.Business
{
    public class MerchantComponent
    {
        /// <summary>
        /// 获取商家实体[token filter用到]
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public MerchantAccount GetMerchantAccountByToken(string token)
        {
            MerchantAccount account = null;
            string merchantId = string.Empty;
            WebRedis.GetWebTokenIndRedis(token, out merchantId);
            var result = Guid.TryParse(merchantId, out Guid guid);
            if (result)
            {
                account = new MerchantAccountDAC().GetById(guid);
                if (account == null)
                    throw new CommonException(ReasonCode.UNAUTHORIZED, "未授权");
                if (!account.POSId.HasValue)
                    throw new CommonException(ReasonCode.ACCOUNT_UNBUNDLED, "帐号已被解绑");

                if (account.Status == AccountStatus.Locked)
                    throw new CommonException(ReasonCode.ACCOUNT_LOCKED, "账户已锁定");
            }
            return account;
        }

        /// <summary>
        /// 获取登录二维码
        /// </summary>
        /// <returns></returns>
        public string GetLoginQRcode()
        {
            string qrCodeKey = WebConfig.QRCodePrefix + Guid.NewGuid().ToString("N");
            //写入redis
            bool reuslt = WebRedis.SetLoginQRCodeIndRedis(qrCodeKey, "0");

            return qrCodeKey;
        }

        /// <summary>
        /// 获取登录数据[轮询]
        /// </summary>
        /// <returns>0=还没有扫码 -1=qrcode已失效 -2=创建token失败 1=成功</returns>
        public int GetLoginData(string qrcode, out string webToken)
        {
            webToken = string.Empty;
            //获取userName
            string merchantUserName = WebRedis.GetLoginQRCodeIndRedis(qrcode);

            if (string.IsNullOrEmpty(merchantUserName))
                return -1;

            if (merchantUserName == "0")//表示还没有扫码
                return 0;

            bool result = WebRedis.SetWebTokenIndRedis(merchantUserName, out webToken);
            if (result)
            {
                WebRedis.DeleteLoginQRCodeIndRedis(qrcode);
            }
            return result ? 1 : -2;
        }


        /// <summary>
        /// POS机扫码登录Web[更新redis的key=qrcode,value=token]
        /// </summary>
        /// <param name="musername">apptoken</param>
        /// <param name="qrcode">二维码字符串</param>
        /// <returns>返回成功/失败</returns>
        public bool ScanQRLogin(string musername, string qrcode)
        {
            if (string.IsNullOrEmpty(musername) || string.IsNullOrEmpty(qrcode))
                return false;

            //填充qrcode存在的token
            bool result = WebRedis.SetLoginQRCodeIndRedis(qrcode, musername);
            return result;
        }

        /// <summary>
        /// 获取商家账户信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public MerchantAccount GetMerchantAccount(Guid merchantId)
        {
            var account = new MerchantAccountDAC().GetById(merchantId);
            if (account == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "用户不存在");
            return account;
        }

        /// <summary>
        /// 获取商家基本信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public MerchantBaseInfo GetMerchantBaseInfo_Web(Guid merchantId)
        {
            var info = new MerchantAccountDAC().GetMerchantBaseInfo_Web(merchantId);
            if (info == null)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "用户不存在");
            }
            var country = new CountryComponent().GetById(info.CountryId);
            info.CountryName = country.Name;
            info.CountryName_CN = country.Name_CN;
            info.IsExistMerchantInfo = new MerchantInformationDAC().GetByMerchantAccountId(info.Id) != null;
            return info;
        }

        /// <summary>
        /// 获取全局数据库的商家个人信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public MerchantProfile GetMerchantProfile(Guid merchantId)
        {
            MerchantProfileAgent agent = new MerchantProfileAgent();
            MerchantProfile profile = agent.GetMerchantProfile(merchantId);
            if (profile == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "当前商家个人资料不存在");
            return profile;
        }


        /// <summary>
        /// 修改商家执照认证
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="businessLicense"></param>
        /// <returns>-1=修改失败 -2=已认证 -3=审核中 1=修改成功</returns>
        public void UpdateMerchantLicense(Guid merchantId, string companyName, string licenseNo, Guid businessLicense)
        {
            MerchantProfileAgent agent = new MerchantProfileAgent();
            MerchantProfile profile = agent.GetMerchantProfile(merchantId);
            if (profile.L2VerifyStatus == VerifyStatus.Certified || profile.L2VerifyStatus == VerifyStatus.UnderApproval)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.VERIFYED_STATUS, "已审核的用户不能更改该信息");
            agent.UpdateMerchantLicense(merchantId, companyName, licenseNo, businessLicense);

        }


        /// <summary>
        /// 修改商家部分account和profiles
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="merchantName"></param>
        /// <param name="email"></param>
        /// <param name="cityId"></param>
        /// <param name="postCode"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <returns></returns>
        public void UpdateMerchantProfiles(Guid merchantId, string merchantName, string email, int cityId, string postCode, string address1)
        {
            MerchantProfileAgent agent = new MerchantProfileAgent();
            agent.UpdateMerchantProfiles(merchantId, merchantName, email, postCode, address1, "");
        }

        /// <summary>
        /// 修改商家头像
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public void UpdateMerchantHeadImage(Guid merchantId, string photoId)
        {
            MerchantProfileAgent agent = new MerchantProfileAgent();
            new MerchantAccountDAC().UpdateMerchantHeadImage(merchantId, photoId);

        }

        /// <summary>
        /// 修改商家名称
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="merchantName"></param>
        /// <returns></returns>
        public void UpdateMerchantName(Guid merchantId, string merchantName)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            accountDAC.UpdateMerchantName(merchantId, merchantName);
        }
        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public void UpdateEmail(Guid merchantId, string email)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            accountDAC.UpdateEmail(merchantId, email);
        }
        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="postCode"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <returns></returns>
        public void UpdateAddress(Guid merchantId, string postCode, string address, string state, string city)
        {
            MerchantProfile model = new MerchantProfile();
            model.Address1 = address;
            model.MerchantId = merchantId;
            model.Postcode = postCode;
            model.City = city;
            model.State = state;

            MerchantProfileAgent agent = new MerchantProfileAgent();
            agent.ModifyAddress1(model);
        }
        /// <summary>
        /// 修改邀请码
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public void UpdateInviterCode(Guid merchantId, string inviterCode)
        {
            UserAccountDAC uaDac = new UserAccountDAC();
            if (!string.IsNullOrEmpty(inviterCode) && !uaDac.ExistInviterCode(inviterCode))
                throw new CommonException(ReasonCode.FiiiPosReasonCode.INVITERCODE_NOT_EXISTS, "邀请码不存在");
            var user = uaDac.GetUserAccountByInviteCode(inviterCode);
            InviteRecordDAC irDac = new InviteRecordDAC();
            InviteRecord model = new InviteRecord();
            model.AccountId = merchantId;
            model.InviterCode = inviterCode;
            model.Type = InviteType.Fiiipos;
            model.Timestamp = DateTime.UtcNow;
            model.InviterAccountId = user.Id;
            irDac.Insert(model); ;
        }
        /// <summary>
        /// 获取POS信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public POS GetPosInfo(Guid merchantId)
        {
            MerchantAccount account = new MerchantAccountDAC().GetById(merchantId);
            return new POSDAC().GetById(account.POSId.Value);
        }

        /// <summary>
        /// 查询是否商家已认证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool SelectId(Guid Id)
        {
            return new MerchantAccountDAC().SelectId (Id) != null;
        }
    }
}
