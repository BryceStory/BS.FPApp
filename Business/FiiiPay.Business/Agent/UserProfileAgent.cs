using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Data.Agents.RPC;
using FiiiPay.DTO;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Data.Agents.APP
{
    public class UserProfileAgent
    {
        public List<MerchantVerifyStatusOM> GetVerifyStatusListByIds(int countryId, List<Guid> ids)
        {
            List<MerchantVerifyStatusOM> result = null;
            List<MerchantProfile> merchants = null;
            var server = ProfileFactory.GetByCountryId(countryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                UserProfileRPC dac = new UserProfileRPC(server);
                merchants = dac.GetMerchantListByIds(ids);
            }
            result = merchants.Select(p => new MerchantVerifyStatusOM
            {
                MerchantId = p.MerchantId,
                Status = p.L2VerifyStatus
            }).ToList();
            return result;
        }

        /// <summary>
        /// 修改性别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0为女;1为男</param>
        /// <returns></returns>
        public bool UpdateGender(Guid id, int type)
        {
            var result = false;
            var server = QueryKYCRouter(id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            result = dac.SetGenderById(id, type);
            return result;
        }

        public bool UpdateLv1Info(Lv1Info im)
        {
            var result = false;
            var server = QueryKYCRouter(im.Id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            result = dac.UpdateLv1Info(im);
            return result;
        }

        public bool UpdateLv2Info(Lv2Info im)
        {
            var result = false;
            var server = QueryKYCRouter(im.Id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            result = dac.UpdateLv2Info(im);
            return result;
        }

        /// <summary>
        /// 获得身份验证状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserVerifiedStatus GetVerifiedStatus(Guid id)
        {
            UserVerifiedStatus result = null;
            var server = QueryKYCRouter(id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            result = dac.GetVerifiedStatus(id);
            return result;
        }

        /// <summary>
        /// 使用UserId获得KYC服务器地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ProfileRouter QueryKYCRouter(Guid? id)
        {
            var dac = new UserAccountDAC();
            var userAccount = dac.GetById(id.Value);
            return ProfileFactory.GetByCountryId(userAccount.CountryId);
        }

        public bool UpdatePhoneNumber(Guid userAccountId, string cellphone)
        {
            var result = false;
            var router = QueryKYCRouter(userAccountId);
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(router);
            result = dac.UpdatePhoneNumber(userAccountId, cellphone);
            return result;
        }

        /// <summary>
        /// 更新L1身份认证状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verifyStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateL1Status(Guid id, VerifyStatus verifyStatus, string remark)
        {
            var result = false;
            var router = QueryKYCRouter(id);
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(router);
            result = dac.UpdateL1Status(id, (int)verifyStatus, remark);
            return result;
        }

        /// <summary>
        /// 更新L2身份认证状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verifyStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateL2Status(Guid id, VerifyStatus verifyStatus, string remark)
        {
            var result = false;
            var router = QueryKYCRouter(id);
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(router);
            result = dac.UpdateL2Status(id, (int)verifyStatus, remark);
            return result;
        }

        /// <summary>
        ///  查询用户状态列表
        /// </summary>
        /// <param name="cellphone"></param>
        /// <param name="country"></param>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <param name="index"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<UserAccountStatus> GetUserAccountStatusList(string cellphone, int country, int? status, int pageSize, int index, out int totalCount)
        {
            var routerDAC = new ProfileRouterDAC();
            var list = new List<UserAccountStatus>();

            var accountDAC = new UserAccountDAC();
            var accountList = accountDAC.GetUserAccountStatusList(cellphone, country, status, pageSize, index, out totalCount);
            var guids = new List<Guid>();

            if (accountList != null)
            {
                foreach (var ac in accountList)
                {
                    guids.Add(ac.Id);
                }
            }
            else
            {
                return null;
            }

            //UserLoginLogDAC logDAC = new UserLoginLogDAC();
            //List<UserLoginLog> logs = logDAC.GetLastLoginTimeListByIds(guids);

            var server = routerDAC.GetRouter(country);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            var profileList = dac.GetListByIds(guids);
            foreach (var account in accountList)
            {
                //UserLoginLog log = null;
                var accountStatus = new UserAccountStatus
                {
                    UserAccountId = account.Id,
                    IsAllowExpense = account.IsAllowExpense,
                    IsAllowWithdrawal = account.IsAllowWithdrawal,
                    Cellphone = account.Cellphone,
                    Country = account.CountryId,
                    RegistrationDate = account.RegistrationDate,
                    Status = account.Status
                };
                UserProfile profile = null;
                if (profileList != null)
                {
                    foreach (var item in profileList)
                    {
                        if (item.UserAccountId == account.Id)
                        {
                            profile = item;
                            break;
                        }
                    }
                }
                if (profile != null)
                {
                    accountStatus.L1VerifyStatus = profile.L1VerifyStatus;
                    accountStatus.L2VerifyStatus = profile.L2VerifyStatus;
                    //accountStatus.Remark = profile.Remark;
                }

                //if (logs != null)
                //{
                //    foreach (var item in logs)
                //    {
                //        if (item.UserAccountId == account.Id)
                //        {
                //            log = item;
                //            break;
                //        }
                //    }
                //}
                //if (log != null)
                //{
                //    accountStatus.LastLoginTimeStamp = log.Timestamp;
                //}

                list.Add(accountStatus);
            }
            return list;
        }

        public List<UserProfile> GetUserProfileListForL1(string cellphone, int country, string orderByFiled, bool isDesc, int? L1VerifyStatus, int pageSize, int index, out int totalCount)
        {

            var routerDAC = new ProfileRouterDAC();
            var server = routerDAC.GetRouter(country);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            return dac.GetUserProfileListForL1(cellphone, country, orderByFiled, isDesc, L1VerifyStatus, pageSize, index, out totalCount);
        }

        public List<UserProfile> GetUserProfileListForL2(string cellphone, int country, string orderByFiled, bool isDesc, int? L2VerifyStatus, int pageSize, int index, out int totalCount)
        {

            var routerDAC = new ProfileRouterDAC();
            var server = routerDAC.GetRouter(country);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            return dac.GetUserProfileListForL2(cellphone, country, orderByFiled, isDesc, L2VerifyStatus, pageSize, index, out totalCount);
        }
        public UserProfile GetUserProfile(Guid id)
        {
            UserProfile result = null;
            var server = QueryKYCRouter(id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            result = dac.GetById(id);
            return result;
        }
        public UserProfileSet GetUserProfileSet(Guid id)
        {
            var profileSet = new UserProfileSet();
            UserAccountDAC userAccountDAC = new UserAccountDAC();
            UserAccount userAccount = userAccountDAC.GetById(id);
            if (userAccount == null)
            {
                return null;//查无
            }
            //赋值
            profileSet.Id = userAccount.Id;
            profileSet.Cellphone = userAccount.Cellphone;
            profileSet.Email = userAccount.Email;
            profileSet.IsVerifiedEmail = userAccount.IsVerifiedEmail;
            profileSet.RegistrationDate = userAccount.RegistrationDate;
            profileSet.CountryId = userAccount.CountryId;
            profileSet.Photo = userAccount.Photo;
            profileSet.Password = userAccount.Password;
            profileSet.Pin = userAccount.Pin;
            profileSet.SecretKey = userAccount.SecretKey;
            profileSet.Status = userAccount.Status;
            profileSet.IsAllowWithdrawal = userAccount.IsAllowWithdrawal;
            profileSet.IsAllowExpense = userAccount.IsAllowExpense;
            profileSet.FiatCurrency = userAccount.FiatCurrency;
            profileSet.InvitationCode = userAccount.InvitationCode;
            profileSet.InviterCode = userAccount.InviterCode;
            profileSet.ValidationFlag = userAccount.ValidationFlag;
            profileSet.AuthSecretKey = userAccount.AuthSecretKey;


            var routerDAC = new ProfileRouterDAC();
            var server = routerDAC.GetRouter(userAccount.CountryId);
            UserProfile userProfile = null;
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var userProfileDAC = new UserProfileRPC(server);
            userProfile = userProfileDAC.GetById(userAccount.Id);
            if (userProfile != null)
            {
                profileSet.UserAccountId = userProfile.UserAccountId;
                profileSet.LastName = userProfile.LastName;
                profileSet.FirstName = userProfile.FirstName;
                //profileSet.Fullname = userProfile.Fullname;
                profileSet.IdentityDocNo = userProfile.IdentityDocNo;
                profileSet.IdentityDocType = userProfile.IdentityDocType;
                //profileSet.IdentityDocFile = userProfile.IdentityDocFile;
                //profileSet.IsIdentityDocVerified = userProfile.IsIdentityDocVerified;
                profileSet.IdentityExpiryDate = userProfile.IdentityExpiryDate;
                profileSet.DateOfBirth = userProfile.DateOfBirth;
                profileSet.Address1 = userProfile.Address1;
                profileSet.Address2 = userProfile.Address2;
                profileSet.City = userProfile.City;
                profileSet.State = userProfile.State;
                profileSet.Postcode = userProfile.Postcode;
                profileSet.Country = userProfile.Country;
                profileSet.Gender = userProfile.Gender;
                profileSet.FrontIdentityImage = userProfile.FrontIdentityImage;
                profileSet.ResidentImage = userProfile.ResidentImage;
                profileSet.BackIdentityImage = userProfile.BackIdentityImage;
                profileSet.HandHoldWithCard = userProfile.HandHoldWithCard;
                profileSet.L1VerifyStatus = userProfile.L1VerifyStatus;
                profileSet.L2VerifyStatus = userProfile.L2VerifyStatus;
                profileSet.L1SubmissionDate = userProfile.L1SubmissionDate;
                profileSet.L2SubmissionDate = userProfile.L2SubmissionDate;
                profileSet.L1Remark = userProfile.L1Remark;
                profileSet.L2Remark = userProfile.L2Remark;

            }
            return profileSet;
        }

        /// <summary>
        /// Updates the birthday.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <exception cref="InvalidProfileServiceException"></exception>
        public bool UpdateBirthday(UserAccount account, DateTime date)
        {
            if (account == null)
            {
                return false;
            }
            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            return dac.UpdateBirthday(account.Id, date);
        }
        /// <summary>
        /// 添加Profile
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public bool AddProfile(UserProfile userProfile)
        {
            if (string.IsNullOrEmpty(userProfile.Cellphone))
            {
                return false;
            }
            if (userProfile.Country == 0)
            {
                throw new Exception("必须指定Profile的国家字段");
            }
            var server = ProfileFactory.GetByCountryId(userProfile.Country.Value);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            return dac.AddProfile(userProfile);
        }
        /// <summary>
        /// 删除Profile
        /// </summary>
        /// <param name="userProfile">必须指定UserAccountId和Country</param>
        /// <returns></returns>
        public bool RemoveProfile(UserProfile userProfile)
        {
            if (userProfile.Country == null)
            {
                return false;
            }

            var server = ProfileFactory.GetByCountryId(userProfile.Country.Value);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            return dac.RemoveProfile(userProfile);
        }

        public int GetCountByIdentityDocNo(Guid id, string IdentityDocNo)
        {
            var server = QueryKYCRouter(id);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }

            var dac = new UserProfileRPC(server);
            var result = dac.GetCountByIdentityDocNo(IdentityDocNo);
            return result;
        }
    }
}
