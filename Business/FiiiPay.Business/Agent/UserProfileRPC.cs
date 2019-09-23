using System;
using System.Collections.Generic;
using FiiiPay.DTO;
using FiiiPay.DTO.Account;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.RPC
{
    public class UserProfileRPC
    {
        private readonly ProfileRouter server;
        private readonly string baseAddress;
        private readonly Dictionary<string, string> headers;

        private readonly ILog _log = LogManager.GetLogger(typeof(UserProfileRPC));

        public UserProfileRPC(ProfileRouter server)
        {
            this.server = server;
            baseAddress = $"{server.ServerAddress}/User";
            headers = new Dictionary<string, string>();
            var token = GenerateToken();
            headers.Add("Authorization", "Bearer " + token);
        }

        public UserProfile GetById(Guid id)
        {
            var url = $"{baseAddress}/GetById";
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
            _log.Error($"RPC get by id={id} result " + result);
            throw new CommonException(10000, data.Message);
        }

        public  bool UpdatePhoneNumber(Guid id, string cellphone)
        {
            UpdatePhoneNumberIM input = new UpdatePhoneNumberIM() {  Id=id,Cellphone=cellphone};
            var url = $"{baseAddress}/UpdatePhoneNumber";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(input));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public List<UserProfile> GetUserProfileListForL1(string cellphone, int country, string orderByFiled, bool isDesc, int? l1VerifyStatus, int pageSize, int index, out int totalCount)
        {
            UserProfileListIM input = new UserProfileListIM();
            input.Cellphone = cellphone;
            input.Country = country;
            input.OrderByFiled = orderByFiled;
            input.IsDesc = isDesc;
            input.VerifyStatus = l1VerifyStatus;
            input.PageSize = pageSize;
            input.Index = index;

            var url = $"{baseAddress}/GetUserProfileListForL1";
    
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(input));
            var data = JsonConvert.DeserializeObject<ServiceResult<UserProfileListOM>>(result);
            if (data.Code == 0)
            {
                totalCount = data.Data.TotalCount;
                return data.Data.ResultSet;
            }
            throw new CommonException(10000, data.Message);
        }
        public List<UserProfile> GetUserProfileListForL2(string cellphone, int country, string orderByFiled, bool isDesc, int? l2VerifyStatus, int pageSize, int index, out int totalCount)
        {
            UserProfileListIM input = new UserProfileListIM();
            input.Cellphone = cellphone;
            input.Country = country;
            input.OrderByFiled = orderByFiled;
            input.IsDesc = isDesc;
            input.VerifyStatus = l2VerifyStatus;
            input.PageSize = pageSize;
            input.Index = index;
            var url = $"{baseAddress}/GetUserProfileListForL2";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(input));
            var data = JsonConvert.DeserializeObject<ServiceResult<UserProfileListOM>>(result);
            if (data.Code == 0)
            {
                totalCount = data.Data.TotalCount;
                return data.Data.ResultSet;
            }
            throw new CommonException(10000, data.Message);
        }

        public List<UserProfile> GetListByIds(List<Guid> guids)
        {
            var url = $"{baseAddress}/GetListByIds";
            var obj = new
            {
                Ids = guids
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<List<UserProfile>>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateLv2Info(Lv2Info im)
        {
            var url = $"{baseAddress}/UpdateLv2Info";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(im));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool SetGenderById(Guid id, int type)
        {
            var url = $"{baseAddress}/SetGenderById";
            var obj = new
            {
                Id = id,
                Type = type
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateIdImage(Guid id, Guid frontImage, Guid backImage, Guid handHoldImage)
        {
            var url = $"{baseAddress}/UpdateIdImage";
            var obj = new
            {
                Id = id,
                FrontImage = frontImage,
                BackImage = backImage,
                HandHoldImage = handHoldImage
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateLv1Info(Lv1Info im)
        {
            var url = $"{baseAddress}/UpdateLv1Info";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(im));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateL1Status(Guid id, int verifyStatus, string remark)
        {
            var url = $"{baseAddress}/UpdateL1Status";
            var obj = new
            {
                Id = id,
                VerifyStatus = verifyStatus,
                Remark = remark
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public UserVerifiedStatus GetVerifiedStatus(Guid id)
        {
            var url = $"{baseAddress}/GetVerifiedStatus";
            var obj = new
            {
                Id = id
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<UserVerifiedStatus>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        
        public bool UpdateL2Status(Guid id, int verifyStatus, string remark)
        {
            var url = $"{baseAddress}/UpdateL2Status";
            var obj = new
            {
                Id = id,
                VerifyStatus= verifyStatus,
                Remark= remark
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateBirthday(Guid id, DateTime date)
        {
            var url = $"{baseAddress}/UpdateBirthday";
            var obj = new
            {
                Id = id,
                Date = date           
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool AddProfile(UserProfile userProfile)
        {
            var url = $"{baseAddress}/AddProfile";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(userProfile));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool RemoveProfile(UserProfile userProfile)
        {
            var url = $"{baseAddress}/RemoveProfile";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(userProfile));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        private string GenerateToken()
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + server.ClientKey;
            var token = AES128.Encrypt(password, server.SecretKey);

            return token;
        }

        public List<MerchantProfile> GetMerchantListByIds(List<Guid> ids)
        {
            var url = $"{baseAddress}/GetListByIds";
            var obj = new GuidsIM
            {
                Guids = ids
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<List<MerchantProfile>>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public int GetCountByIdentityDocNo(string IdentityDocNo)
        {
            var url = $"{baseAddress}/GetCountByIdentityDocNo";
            var obj = new
            {
                IdentityDocNo = IdentityDocNo
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<int>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            _log.Error($"RPC get by id={IdentityDocNo} result " + result);
            throw new CommonException(10000, data.Message);
        }
    }
}
