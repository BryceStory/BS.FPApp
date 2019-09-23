using FiiiPay.Profile.API.Common;
using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPay.Profile.API.Models;
using FiiiPay.Profile.API.Form;
using FiiiPay.Profile.Data;
using FiiiPay.Profile.Entities;
using log4net;

namespace FiiiPay.Profile.API.Controllers
{
    [RoutePrefix("Merchant")]
    [Authorize]
    public class MerchantController : ApiController
    {
        [HttpPost]
        [Route("AddMerchant")]
        public ServiceResult<bool> AddMerchant(MerchantProfile profile)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.Insert(profile));
        }
        [HttpPost]
        [Route("RemoveMerchant")]
        public ServiceResult<bool> RemoveMerchant(MerchantProfile profile)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.Delete(profile.MerchantId));
        }
        [HttpPost]
        [Route("UpdateL2VerifyStatus")]
        public ServiceResult<bool> UpdateL2VerifyStatus(UpdateVerifyStatusIM im)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.UpdateL2VerifyStatus(im.Id, im.VerifyStatus, im.Remark));
        }

        [HttpPost]
        [Route("UpdateL1VerifyStatus")]
        public ServiceResult<bool> UpdateL1VerifyStatus(UpdateVerifyStatusIM im)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.UpdateL1VerifyStatus(im.Id, im.VerifyStatus, im.Remark));
        }

        [HttpPost]
        [Route("UpdateLicenseInfo")]
        public ServiceResult<bool> UpdateLicenseInfo(MerchantLicenseInfo form)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.UpdateLicenseInfo(form));
        }
        [HttpPost]
        [Route("GetById")]
        public ServiceResult<MerchantProfile> GetById(MerchantLicenseInfo form)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.GetById(form.Id));
        }
        [HttpPost]
        [Route("GetListByIds")]
        public ServiceResult<List<MerchantProfile>> GetListByIds(GuidsIM form)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            return ResultHelper.OKResult(dac.GetListByIds(form.Guids));
        }
        [HttpPost]
        [Route("ModifyAddress1")]
        public ServiceResult<bool> ModifyAddress1(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.ModifyAddress1(profile);
            return ResultHelper.OKResult(result);
        }

        /// <summary>
        /// 根据 MerchantId 修改 Address2
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ModifyAddress2")]
        public ServiceResult<bool> ModifyAddress2(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.ModifyAddress2(profile);
            return ResultHelper.OKResult(result);
        }



        /// <summary>
        /// 提价营业执照
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateMerchantLicense")]
        public ServiceResult<bool> UpdateMerchantLicense(UpdateMerchantLicenseIM profile)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            bool result = dac.UpdateMerchantLicense(profile.MerchantId, profile.CompanyName, profile.LicenseNo, profile.BusinessLicense);
            return ResultHelper.OKResult(result);
        }

        [HttpPost]
        [Route("UpdateAddress")]
        public ServiceResult<bool> UpdateAddress(Address address)
        {
            MerchantProfileDAC merchantProfileDAC = new MerchantProfileDAC();
            bool result = merchantProfileDAC.UpdateAddress(address.Id, address.PostCode, address.State,
                address.City, address.Address1, address.Address2);
            return ResultHelper.OKResult(result);
        }

        /// <summary>
        /// 提价营业执照
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CommitBusinessLicense")]
        public ServiceResult<bool> CommitBusinessLicense(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.CommitBusinessLicense(profile);
            return ResultHelper.OKResult(result);
        }
        /// <summary>
        /// BO查询L1审核列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMerchantVerifyListL1")]
        public ServiceResult<GetMerchnatVerifyListOM> GetMerchantVerifyListL1(GetMerchnatVerifyListIM input)
        {
            int totalCount = 0;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            GetMerchnatVerifyListOM output = new GetMerchnatVerifyListOM();
            output.ResultSet = dac.GetMerchantVerifyListL1(input.cellphone, input.countryId, input.status, input.orderByFiled, input.isDesc, input.pageSize, input.index, out totalCount);
            output.TotalCount = totalCount;
            return ResultHelper.OKResult(output);
        }
        /// <summary>
        /// BO查询L2审核列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMerchantVerifyListL2")]
        public ServiceResult<GetMerchnatVerifyListOM> GetMerchantVerifyListL2(GetMerchnatVerifyListIM input)
        {
            int totalCount = 0;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            GetMerchnatVerifyListOM output = new GetMerchnatVerifyListOM();
            output.ResultSet = dac.GetMerchantVerifyListL2(input.cellphone, input.countryId, input.status, input.orderByFiled, input.isDesc, input.pageSize, input.index, out totalCount);
            output.TotalCount = totalCount;
            return ResultHelper.OKResult(output);
        }

        /// <summary>
        ///  根据 MerchantId 修改 Fullname
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ModifyFullname")]
        public ServiceResult<bool> ModifyFullname(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.ModifyFullname(profile);
            return ResultHelper.OKResult(result);
        }

        /// <summary>
        ///  根据 MerchantId 修改 Cellphone
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateCellphone")]
        public ServiceResult<bool> UpdateCellphone(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.ModifyCellphone(profile);
            return ResultHelper.OKResult(result);
        }

        /// <summary>
        ///  根据 MerchantId 修改 Identity
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ModifyIdentity")]
        public ServiceResult<bool> ModifyIdentity(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.ModifyIdentity(profile);
            return ResultHelper.OKResult(result);
        }

        /// <summary>
        /// 提交商家身份证正反面
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CommitIdentityImage")]
        public ServiceResult<bool> CommitIdentityImage(MerchantProfile profile)
        {
            bool result = false;
            MerchantProfileDAC dac = new MerchantProfileDAC();
            result = dac.CommitIdentityImage(profile);
            return ResultHelper.OKResult(result);
        }

        [HttpPost]
        [Route("GetCountByIdentityDocNo")]
        public ServiceResult<int> GetCountByIdentityDocNo(FiiiPosIdentityDocNo form)
        {
            MerchantProfileDAC dac = new MerchantProfileDAC();
            int result = dac.GetCountByIdentityDocNo(form.IdentityDocNo);
            return ResultHelper.OKResult(result);
        }
    }
}
