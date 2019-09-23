using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Data.Agents.RPC;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPOS.Business;
using FiiiPOS.DTO;
using log4net;

namespace FiiiPay.Data.Agents.APP
{
    public class MerchantProfileAgent
    {
        public bool RemoveMerchantById(MerchantProfile profile)
        {
            if (profile.Country == 0)
            {
                throw new Exception("必须设置Profile的国家字段");
            }
            var server = ProfileFactory.GetByCountryId(profile.Country);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.Delete(profile);
            }
        }
        public bool AddMerchant(MerchantProfile profile)
        {
            var server = ProfileFactory.GetByCountryId(profile.Country);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.Insert(profile);
            }
        }
        /// <summary>
        ///根据 MerchantId 修改 Address1,Postcode,City,State
        /// </summary>
        /// <param name="profile"></param>
        public bool ModifyAddress1(MerchantProfile profile)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(profile.MerchantId);
            if (account == null)
            {
                return false;
            }
            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.ModifyAddress1(profile);
            }

        }

        public bool ModifyFullname(MerchantProfile profile)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(profile.MerchantId);
            if (account == null)
            {
                return false;
            }
            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.ModifyFullname(profile);
            }

        }

        public bool ModifyIdentity(MerchantProfile profile)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(profile.MerchantId);
            if (account == null)
            {
                return false;
            }
            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.ModifyIdentity(profile);
            }

        }

        /// <summary>
        ///提交营业执照
        /// BusinessLicenseImage  LicenseNo LinceseName,VerifyStatus
        /// </summary>
        public bool CommitBusinessLicense(MerchantProfile profile)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(profile.MerchantId);
            if (account == null)
            {
                return false;
            }

            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.CommitBusinessLicense(profile);
            }
        }
        /// <summary>
        ///提交商户个人信息
        /// </summary>
        public bool CommitIdentityImage(MerchantProfile profile)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(profile.MerchantId);
            if (account == null)
            {
                return false;
            }

            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.CommitIdentityImage(profile);
            }
        }


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
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                merchants = dac.GetListByIds(ids);
            }
            result = merchants.Select(p => new MerchantVerifyStatusOM
            {
                MerchantId = p.MerchantId,
                Status = p.L2VerifyStatus
            }).ToList();
            return result;
        }

        public List<MerchantProfile> GetMerchantVerifyListL2(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {

            if (string.IsNullOrEmpty(orderByFiled))
            {
                throw new Exception("orderByFiled 不能为空");
            }
            List<MerchantProfile> result = null;
            totalCount = 0;
            var server = ProfileFactory.GetByCountryId(countryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                result = dac.GetMerchantVerifyListL2(cellphone, countryId, status, orderByFiled, isDesc, pageSize, index, out totalCount);
            }

            return result;
        }

        public List<MerchantProfile> GetMerchantVerifyListForL1(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {

            if (string.IsNullOrEmpty(orderByFiled))
            {
                throw new Exception("orderByFiled 不能为空");
            }
            List<MerchantProfile> result = null;
            totalCount = 0;
            var server = ProfileFactory.GetByCountryId(countryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                result = dac.GetMerchantVerifyListL1(cellphone, countryId, status, orderByFiled, isDesc, pageSize, index, out totalCount);
            }

            return result;
        }

        #region 商家Web版业务
        /// <summary>
        /// 商家Web版修改认证信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="companyName"></param>
        /// <param name="licenseNo"></param>
        /// <param name="businessLicense"></param>
        /// <returns></returns>
        public bool UpdateMerchantLicense(Guid merchantId, string companyName, string licenseNo, Guid businessLicense)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(merchantId);
            if (account == null)
            {
                return false;
            }
            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                MerchantProfileDAC dac = new MerchantProfileDAC();
                return dac.UpdateMerchantLicense(merchantId, companyName, licenseNo, businessLicense);
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(server);
                return dac.UpdateMerchantLicense(merchantId, companyName, licenseNo, businessLicense);
            }
        }

        /// <summary>
        /// 修改商家Web部分account和profiles
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="merchantName"></param>
        /// <param name="email"></param>
        /// <param name="postCode"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <returns></returns>
        public bool UpdateMerchantProfiles(Guid merchantId, string merchantName, string email, string postCode, string address1, string address2)
        {
            try
            {
                MerchantAccountDAC accountDAC = new MerchantAccountDAC();
                accountDAC.UpdateMerchantName(merchantId, merchantName);
                accountDAC.UpdateEmail(merchantId, email);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 修改商家Web头像
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public bool UpdateMerchantHeadImage(Guid merchantId, string photoId)
        {
            MerchantAccountDAC dac = new MerchantAccountDAC();
            return dac.UpdateMerchantHeadImage(merchantId, photoId);
        }
        #endregion

        /// <summary>
        /// 获得商家Profile信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MerchantProfile GetMerchantProfile(Guid id)
        {
            MerchantAccountDAC merchantAccountDAC = new MerchantAccountDAC();
            var merchantAccount = merchantAccountDAC.GetById(id);

            if (merchantAccount == null)
            {
                return null;//查无此人
            }
            var router = ProfileFactory.GetByCountryId(merchantAccount.CountryId);
            MerchantProfile profile = null;
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(router);
                profile = dac.GetById(id);
            }
            return profile;
        }
        /// <summary>
        /// 获得MerchantProfile+Account信息体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MerchantProfileSet GetMerchantProfileSet(Guid id)
        {

            MerchantProfileSet profileSet = new MerchantProfileSet();
            //使用Id查询本库的基本信息
            MerchantAccountDAC merchantAccountDAC = new MerchantAccountDAC();
            var merchantAccount = merchantAccountDAC.GetById(id);
            if (merchantAccount == null)
            {
                return null;//查无此人
            }
            //赋值
            profileSet.Id = merchantAccount.Id;
            profileSet.Cellphone = merchantAccount.Cellphone;
            profileSet.Username = merchantAccount.Username;
            profileSet.MerchantName = merchantAccount.MerchantName;
            //profileSet.IsVerified = merchantAccount.IsVerified;
            profileSet.POSId = merchantAccount.POSId;
            //profileSet.BeaconId = merchantAccount.BeaconId;
            profileSet.Email = merchantAccount.Email;
            profileSet.IsVerifiedEmail = merchantAccount.IsVerifiedEmail;
            profileSet.CountryId = merchantAccount.CountryId;
            profileSet.RegistrationDate = merchantAccount.RegistrationDate;
            profileSet.Photo = merchantAccount.Photo;
            profileSet.PIN = merchantAccount.PIN;
            profileSet.SecretKey = merchantAccount.SecretKey;
            profileSet.IsAllowWithdrawal = merchantAccount.IsAllowWithdrawal;
            profileSet.IsAllowAcceptPayment = merchantAccount.IsAllowAcceptPayment;
            profileSet.FiatCurrency = merchantAccount.FiatCurrency;
            profileSet.AuthSecretKey = merchantAccount.AuthSecretKey;
            profileSet.ValidationFlag = merchantAccount.ValidationFlag;

            //使用基本信息中的国别匹配kyc服务器的位置
            if (merchantAccount.POSId.HasValue)
            {
                POS pos = new POSDAC().GetById(merchantAccount.POSId.Value);
                if (pos != null)
                {
                    profileSet.SN = pos.Sn;
                }
            }
            var server = ProfileFactory.GetByCountryId(merchantAccount.CountryId);
            MerchantProfile merchantProfile = null;
            if (server == null)
            {
                //查询HK数据库
                MerchantProfileDAC merchantProfileDAC = new MerchantProfileDAC();
                //赋值
                merchantProfile = merchantProfileDAC.GetById(id);
            }
            else
            {
                MerchantProfileRPC merchantProfileDAC = new MerchantProfileRPC(server);
                //赋值
                merchantProfile = merchantProfileDAC.GetById(id);

            }
            if (merchantProfile != null)
            {
                profileSet.Address1 = merchantProfile.Address1;
                profileSet.Address2 = merchantProfile.Address2;
                profileSet.City = merchantProfile.City;
                profileSet.L1VerifyStatus = merchantProfile.L1VerifyStatus;
                profileSet.L2VerifyStatus = merchantProfile.L2VerifyStatus;
                profileSet.Postcode = merchantProfile.Postcode;
                profileSet.Country = merchantProfile.Country;
                profileSet.BusinessLicenseImage = merchantProfile.BusinessLicenseImage;
                profileSet.LicenseNo = merchantProfile.LicenseNo;
                profileSet.CompanyName = merchantProfile.CompanyName;
                profileSet.IdentityDocNo = merchantProfile.IdentityDocNo;
                profileSet.IdentityDocType = merchantProfile.IdentityDocType;
                profileSet.FirstName = merchantProfile.FirstName;
                profileSet.LastName = merchantProfile.LastName;
                profileSet.BackIdentityImage = merchantProfile.BackIdentityImage;
                profileSet.FrontIdentityImage = merchantProfile.FrontIdentityImage;
                profileSet.HandHoldWithCard = merchantProfile.HandHoldWithCard;
            }

            return profileSet;
        }

        public bool UpdateL1VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            bool result = false;
            MerchantAccountDAC merchantAccountDAC = new MerchantAccountDAC();
            var merchantAccount = merchantAccountDAC.GetById(id);
            if (merchantAccount == null)
            {
                return result;//查无此人
            }
            var router = ProfileFactory.GetByCountryId(merchantAccount.CountryId);
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(router);
                result = dac.UpdateL1VerifyStatus(id, verifyStatus, remark);
            }
            return result;
        }
        public bool UpdateL2VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            bool result = false;
            MerchantAccountDAC merchantAccountDAC = new MerchantAccountDAC();
            var merchantAccount = merchantAccountDAC.GetById(id);
            if (merchantAccount == null)
            {
                return result;//查无此人
            }
            var router = ProfileFactory.GetByCountryId(merchantAccount.CountryId);
            if (router == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC dac = new MerchantProfileRPC(router);
                result = dac.UpdateL2VerifyStatus(id, verifyStatus, remark);
            }
            return result;
        }
        public bool UpdateMerchantName(Guid merchantId, string merchantName)
        {
            try
            {
                MerchantAccountDAC accountDAC = new MerchantAccountDAC();
                accountDAC.UpdateMerchantName(merchantId, merchantName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool UpdateEmail(Guid merchantId, string email)
        {
            try
            {
                MerchantAccountDAC accountDAC = new MerchantAccountDAC();
                accountDAC.UpdateEmail(merchantId, email);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool UpdateAddress(Guid merchantId, Address address)
        {
            try
            {
                MerchantAccountDAC accountDAC = new MerchantAccountDAC();
                var account = accountDAC.GetById(merchantId);

                var server = ProfileFactory.GetByCountryId(account.CountryId);
                if (server == null)
                {
                    throw new InvalidProfileServiceException();
                }
                else
                {
                    MerchantProfileRPC merchantProfileDAC = new MerchantProfileRPC(server);
                    //赋值
                    return merchantProfileDAC.UpdateAddress(address);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCellphone(MerchantProfile profile)
        {
            try
            {
                var server = ProfileFactory.GetByCountryId(profile.Country);
                if (server == null)
                {
                    throw new InvalidProfileServiceException();
                }
                else
                {
                    MerchantProfileRPC merchantProfileDAC = new MerchantProfileRPC(server);
                    //赋值
                    return merchantProfileDAC.UpdateCellphone(profile);
                }
            }
            catch
            {
                return false;
            }
        }

        public int GetCountByIdentityDocNo(Guid merchantId, string IdentityDocNo)
        {
            MerchantAccountDAC accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(merchantId);

            var server = ProfileFactory.GetByCountryId(account.CountryId);
            if (server == null)
            {
                throw new InvalidProfileServiceException();
            }
            else
            {
                MerchantProfileRPC merchantProfileDAC = new MerchantProfileRPC(server);

                return merchantProfileDAC.GetCountByIdentityDocNo(IdentityDocNo);
            }
        }
    }
}
