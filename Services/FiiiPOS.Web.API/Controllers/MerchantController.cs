using FiiiPOS.Web.API.Base;
using FiiiPOS.Web.API.Models.Output;
using FiiiPOS.Web.Business;
using System;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Framework;
using FiiiPOS.Web.API.Models.Input;
using System.Linq;
using System.Threading.Tasks;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.Web.API.Controllers
{
    /// <summary>
    /// 商户相关接口
    /// </summary>
    [RoutePrefix("api/Merchant")]
    public class MerchantController : BaseApiController
    {
        /// <summary>
        /// 获取商家个人Profile资料
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantProfile")]
        public ServiceResult<MerchantProfileOM> GetMerchantProfile()
        {
            MerchantProfileOM outputModel = null;
            MerchantProfile profile = new MerchantComponent().GetMerchantProfile(WorkContext.MerchantId);
            outputModel = new MerchantProfileOM()
            {
                MerchantId = profile.MerchantId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                IdentityDocType = profile.IdentityDocType,
                IdentityDocNo = profile.IdentityDocNo,
                FrontIdentityImage = profile.FrontIdentityImage,
                BackIdentityImage = profile.BackIdentityImage,
                HandHoldWithCard = profile.HandHoldWithCard,
                LicenseNo = profile.LicenseNo,
                CompanyName = profile.CompanyName,
                Country = profile.Country,
                BusinessLicenseImage = profile.BusinessLicenseImage,
                Postcode = profile.Postcode,
                Address1 = profile.Address1,
                State = profile.State,
                City = profile.City,
                L1VerifyStatus = profile.L1VerifyStatus,
                L1Remark = profile.L1Remark,
                L2VerifyStatus = profile.L2VerifyStatus,
                L2Remark = profile.L2Remark,
            };
            return Result_OK(outputModel);
        }

        /// <summary>
        /// 获取商家基本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantBaseInfo")]
        public ServiceResult<MerchantBaseInfoOM> GetMerchantBaseInfo()
        {
            MerchantBaseInfoOM outputModel = null;
            MerchantBaseInfo entity = new MerchantComponent().GetMerchantBaseInfo_Web(WorkContext.MerchantId);
            outputModel = new MerchantBaseInfoOM()
            {
                Id = entity.Id,
                Username = entity.Username,
                MerchantName = entity.MerchantName,
                Photo = entity.Photo,
                PosSN = entity.PosSN,
                Cellphone = entity.Cellphone,
                CountryName = entity.CountryName,
                CountryName_CN = entity.CountryName_CN,
                Email = entity.Email,
                IsVerifiedEmail = entity.IsVerifiedEmail,
                InviterCode = entity.InviterCode,
                IsExistMerchantInfo = entity.IsExistMerchantInfo
            };
            return Result_OK(outputModel);
        }

        /// <summary>
        /// 获取商家简要信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetMerchantSimpleInfo")]
        public ServiceResult<MerchantSimpleInfoOutModel> GetMerchantSimpleInfo()
        {
            MerchantSimpleInfoOutModel outputModel = null;

            MerchantAccount account = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId);

            outputModel = new MerchantSimpleInfoOutModel()
            {
                Id = account.Id,
                Username = account.Username,
                MerchantName = account.MerchantName,
                PhotoId = account.Photo
            };

            return Result_OK(outputModel);
        }

        /// <summary>
        /// 修改认证信息
        /// </summary>
        /// <returns>0=有属性为空  -1=修改失败 1=修改成功</returns>
        [HttpPost, Route("UpdateMerchantLicense")]
        public ServiceResult UpdateMerchantLicense(UpdateLicenseInModel model)
        {
            new MerchantComponent().UpdateMerchantLicense(WorkContext.MerchantId, model.CompanyName, model.LicenseNo, Guid.Parse(model.BusinessLicense));
            return Result_OK("");
        }

        /// <summary>
        /// 修改商家头像
        /// </summary>
        /// <returns>大于0修改成功 小于等于0=修改失败</returns>
        [HttpPost, Route("UpdateMerchantHeadImage")]
        public ServiceResult UpdateMerchantHeadImage(HeadImageInModel model)
        {
            new MerchantComponent().UpdateMerchantHeadImage(WorkContext.MerchantId, model.PhotoId);
            return Result_OK("");
        }

        /// <summary>
        /// 修改商家名称
        /// </summary>
        /// <returns>-1=有属性为空 -2 修改失败 1=修改成功</returns>
        [HttpPost, Route("UpdateMerchantName")]
        public ServiceResult UpdateMerchantName(UpdateMerchantNameModel model)
        {
            new MerchantComponent().UpdateMerchantName(WorkContext.MerchantId, model.MerchantName);
            return Result_OK("");
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <returns>-1=有属性为空 -2 修改失败 1=修改成功</returns>
        [HttpPost, Route("UpdateEmail")]
        public ServiceResult UpdateEmail(UpdateEmailModel model)
        {
            new MerchantComponent().UpdateEmail(WorkContext.MerchantId, model.Email);
            return Result_OK("");
        }

        /// <summary>
        /// 修改地址
        /// </summary>
        /// <returns>-1=有属性为空 -2 修改失败 1=修改成功</returns>
        [HttpPost, Route("UpdateAddress")]
        public ServiceResult UpdateAddress(UpdateAddressModel model)
        {
            new MerchantComponent().UpdateAddress(WorkContext.MerchantId, model.Postcode, model.Address, model.State, model.City);
            return Result_OK("");
        }

        /// <summary>
        /// 修改邀请码
        /// </summary>
        /// <returns>-1=有属性为空 -2 修改失败 1=修改成功</returns>
        [HttpPost, Route("UpdateInviterCode")]
        public ServiceResult UpdateInviterCode(UpdateInviterCodeModel model)
        {
            new MerchantComponent().UpdateInviterCode(WorkContext.MerchantId, model.InviterCode);
            return Result_OK("");
        }

        /// <summary>
        /// 提交商家个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("CommitIdentityImage")]
        public ServiceResult CommitIdentityImage(CommitIdentityModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }

            var accountId = this.WorkContext.MerchantId;

            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                BackIdentityImage = model.BackIdentityImage,
                FirstName = model.FirstName,
                FrontIdentityImage = model.FrontIdentityImage,
                HandHoldWithCard = model.HandHoldWithCard,
                IdentityDocNo = model.IdentityDocNo,
                IdentityDocType = model.IdentityDocType,
                LastName = model.LastName,
                L1VerifyStatus = VerifyStatus.UnderApproval
            };

            new ProfileComponent().CommitIdentityImage(profile);

            return result;
        }

        /// <summary>
        /// 提交营业执照
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("CommitBusinessLicense")]
        public ServiceResult CommitBusinessLicense(CommitBusinessLicenseModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }

            var accountId = this.WorkContext.MerchantId;
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                LicenseNo = model.Number,
                BusinessLicenseImage = model.Image,
                CompanyName = model.Name
            };

            new ProfileComponent().CommitBusinessLicense(profile);

            return result;
        }

        /// <summary>
        /// 发送邮件验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SendVerifyEmail")]
        public async Task<ServiceResult> SendVerifyEmail(SendVerifyEmailModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }

            var accountId = this.WorkContext.MerchantId;

            await new ProfileComponent().SendVerifyEmail(accountId, model.EmailAddress);
            return result;
        }

        /// <summary>
        /// 验证验证码，修改邮箱
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyEmail")]
        public ServiceResult ModifyEmail(ModifyEmailModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            var accountId = this.WorkContext.MerchantId;
            

            new ProfileComponent().ModifyEmail(accountId, model.Code, AES128.Encrypt(model.Token, AES128.DefaultKey));

            return result;
        }

        /// <summary>
        /// 发送邮件验证码[原邮箱]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SendVerifyOriginalEmail")]
        public async Task<ServiceResult> SendVerifyOriginalEmail(SendVerifyEmailModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.WorkContext.MerchantId;

            await new ProfileComponent().SendVerifyOriginalEmail(accountId, model.EmailAddress);

            return result;
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyOriginalEmail")]
        public ServiceResult VerifyOriginalEmail(VerifyOriginalEmailIM im)
        {
            var result = new ServiceResult();
            var accountId = this.WorkContext.MerchantId;
            new ProfileComponent().VerifyOriginalEmail(accountId, im.Email, im.Code);

            return result;
        }

        /// <summary>
        /// 获取商家个人资料
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetPOSInfo")]
        public ServiceResult<POSInfoOM> GetPOSInfo()
        {
            POSInfoOM outputModel = null;
            MerchantAccount account = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId);
            POS pos = new MerchantComponent().GetPosInfo(WorkContext.MerchantId);
            if (account == null)
                return Result_Fail(outputModel, -1, "当前商家个人资料不存在");

            outputModel = new POSInfoOM()
            {
                POSSN = pos.Sn,
                ModelNumber = "N3",
                ProcessorInfo = "Technologies,insMSM8909",
                ResolutionRatio = "480 x 854",
                AndroidVersion = "5.1.1",
                FirmwareVersion = "v1.5.8-FiiiPOS00000001",
                FactoryDate = pos.Timestamp,
                BasebandVersion = "SC800-V7C.M1.01.01 NV.SS.V1.12",
            };
            return Result_OK(outputModel);
        }
        /// <summary>
        /// 查询商家是否已认证
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SelectId")]
        public ServiceResult<bool> SelectId()
        {
            var accountId = this.WorkContext.MerchantId;
            return Result_OK(new MerchantComponent().SelectId(accountId));
        }
    }
}
