using FiiiPay.Profile.API.Common;
using FiiiPay.Profile.API.Form;
using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPay.Profile.API.Models;
using FiiiPay.Profile.Data;
using FiiiPay.Profile.Entities;

namespace FiiiPay.Profile.API.Controllers
{
    [RoutePrefix("User")]
    [Authorize]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("GetById")]
        public ServiceResult<UserProfile> GetById(FiiiPayId form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.GetById(form.Id));
        }

        [HttpPost]
        [Route("GetListByIds")]
        public ServiceResult<List<UserProfile>> GetListByIds(IdList form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            List<UserProfile> profileList = dac.GetListByIds(form.Ids);
            return ResultHelper.OKResult(profileList);
        }
        [HttpPost]
        [Route("SetGenderById")]
        public ServiceResult<bool> SetGenderById(GenderForm form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.SetGenderById(form.Id, form.Type));
        }
        [HttpPost]
        [Route("UpdateLv2Info")]
        public ServiceResult<bool> UpdateLv2Info(Lv2Info form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.UpdateLv2Info(form));
        }

        [HttpPost]
        [Route("UpdateLv1Info")]
        public ServiceResult<bool> UpdateLv1Info(Lv1Info form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.UpdateLv1Info(form));
        }
        [HttpPost]
        [Route("UpdateIdImage")]
        public ServiceResult<bool> UpdateIdImage(UpdateIdImage model)
        {
            UserProfileDAC dac = new UserProfileDAC();
            bool result = dac.UpdateIdImage(model.Id, model.FrontImage, model.BackImage, model.HandHoldImage);
            return ResultHelper.OKResult(result);
        }
        [HttpPost]
        [Route("UpdateL1Status")]
        public ServiceResult<bool> UpdateL1Status(UpdateStatusIM model)
        {
            UserProfileDAC dac = new UserProfileDAC();
            bool result = dac.UpdateL1Status(model.Id, (int)model.VerifyStatus, model.Remark);
            return ResultHelper.OKResult(result);
        }
        [HttpPost]
        [Route("UpdateL2Status")]
        public ServiceResult<bool> UpdateL2Status(UpdateStatusIM model)
        {
            UserProfileDAC dac = new UserProfileDAC();
            bool result = dac.UpdateL2Status(model.Id, (int)model.VerifyStatus, model.Remark);
            return ResultHelper.OKResult(result);
        }
        
        [HttpPost]
        [Route("AddUser")]
        public ServiceResult<bool> AddUser(UserRegInfo info)
        {
            UserProfileDAC dac = new UserProfileDAC();
            bool result = dac.AddUser(info);
            return ResultHelper.OKResult(result);
        }
        [HttpPost]
        [Route("UpdateBirthday")]
        public ServiceResult<bool> UpdateBirthday(DateIM form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.UpdateBirthday(form.Id, form.Date));
        }
        [HttpPost]
        [Route("AddProfile")]
        public ServiceResult<bool> AddProfile(UserProfile userProfile)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.AddProfile(userProfile));
        }

        [HttpPost]
        [Route("GetUserProfileListForL1")]
        public ServiceResult<UserProfileListOM> GetUserProfileListForL1(UserProfileListIM input)
        {
            UserProfileDAC dac = new UserProfileDAC();
            int totalCount = 0;
            UserProfileListOM output = new UserProfileListOM();
            output.ResultSet = dac.GetUserProfileListForL1(input.Cellphone, input.Country, input.OrderByFiled, input.IsDesc, input.VerifyStatus, input.PageSize, input.Index, out totalCount);
            output.TotalCount = totalCount;
            return ResultHelper.OKResult(output);
        }
        [HttpPost]
        [Route("GetUserProfileListForL2")]
        public ServiceResult<UserProfileListOM> GetUserProfileListForL2(UserProfileListIM input)
        {
            UserProfileDAC dac = new UserProfileDAC();
            int totalCount = 0;
            UserProfileListOM output = new UserProfileListOM();
            output.ResultSet = dac.GetUserProfileListForL2(input.Cellphone, input.Country, input.OrderByFiled, input.IsDesc, input.VerifyStatus, input.PageSize, input.Index, out totalCount);
            output.TotalCount = totalCount;
            return ResultHelper.OKResult(output);
        }
        [HttpPost]
        [Route("UpdatePhoneNumber")]
        public ServiceResult<bool> UpdatePhoneNumber(UpdatePhoneNumberIM input)
        {
            UserProfileDAC dac = new UserProfileDAC();
            bool result = dac.UpdatePhoneNumber(input.Id, input.Cellphone);
            return ResultHelper.OKResult(result);
        }
        [HttpPost]
        [Route("RemoveProfile")]
        public ServiceResult<bool> RemoveProfile(UserProfile profile)
        {
            UserProfileDAC dac = new UserProfileDAC();
            return ResultHelper.OKResult(dac.Delete(profile.UserAccountId.Value));
        }

        [HttpPost]
        [Route("GetCountByIdentityDocNo")]
        public ServiceResult<int> GetCountByIdentityDocNo(FiiiPayIdentityDocNo form)
        {
            UserProfileDAC dac = new UserProfileDAC();
            int result = dac.GetCountByIdentityDocNo(form.IdentityDocNo);
            return ResultHelper.OKResult(result);
        }
    }
}