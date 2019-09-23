using System;
using System.Collections.Generic;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.DTO;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.RPC
{
    public class MerchantProfileRPC
    {
        private readonly ProfileRouter server;
        private readonly string baseAddress;
        private readonly Dictionary<string, string> headers;

        private readonly ILog _log = LogManager.GetLogger(typeof(MerchantProfileRPC));

        public MerchantProfileRPC(ProfileRouter server)
        {
            this.server = server;
            baseAddress = $"{server.ServerAddress}/Merchant";
            //初始化请求头
            headers = new Dictionary<string, string>();
            var token = GenerateToken();
            headers.Add("Authorization", "Bearer " + token);
        }

        public bool UpdateLicenseInfo(MerchantLicenseInfo model)
        {
            var url = $"{baseAddress}/UpdateLicenseInfo";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(model));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool Delete(MerchantProfile profile)
        {
            var url = $"{baseAddress}/RemoveMerchant";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        public List<MerchantProfile> GetMerchantVerifyListL1(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {
            totalCount = 0;
            GetMerchnatVerifyListIM input = new GetMerchnatVerifyListIM();
            input.cellphone = cellphone;
            input.countryId = countryId;
            input.status = status;
            input.orderByFiled = orderByFiled;
            input.isDesc = isDesc;
            input.pageSize = pageSize;
            input.index = index;
            var url = $"{baseAddress}/GetMerchantVerifyListL1";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(input));
            var data = JsonConvert.DeserializeObject<ServiceResult<GetMerchnatVerifyListOM>>(result);
            if (data.Code == 0)
            {
                totalCount = data.Data.TotalCount;
                return data.Data.ResultSet;
            }
            throw new CommonException(10000, data.Message);
        }
        public List<MerchantProfile> GetMerchantVerifyListL2(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {
            totalCount = 0;
            GetMerchnatVerifyListIM input = new GetMerchnatVerifyListIM();
            input.cellphone = cellphone;
            input.countryId = countryId;
            input.status = status;
            input.orderByFiled = orderByFiled;
            input.isDesc = isDesc;
            input.pageSize = pageSize;
            input.index = index;

            var url = $"{baseAddress}/GetMerchantVerifyListL2";
            var param = JsonConvert.SerializeObject(input);

            _log.Info("GetMerchantVerifyListL2 url = " + url);
            _log.Info("GetMerchantVerifyListL2 input = " + param);
            _log.Info("GetMerchantVerifyListL2 headers = " + JsonConvert.SerializeObject(headers));

            var result = RestUtilities.PostJson(url, headers, param);
            _log.Info("GetMerchantVerifyListL2 result = " + result);
            var data = JsonConvert.DeserializeObject<ServiceResult<GetMerchnatVerifyListOM>>(result);
            if (data.Code == 0)
            {
                totalCount = data.Data.TotalCount;
                return data.Data.ResultSet;
            }
            throw new CommonException(10000, data.Message);
        }

        internal bool ModifyAddress1(MerchantProfile profile)
        {
            var url = $"{baseAddress}/ModifyAddress1";
            var paramStr = JsonConvert.SerializeObject(profile);
            var result = RestUtilities.PostJson(url, headers, paramStr);

            _log.Info($"url: {url},param: {paramStr},  result: {result}");

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        internal bool ModifyFullname(MerchantProfile profile)
        {
            var url = $"{baseAddress}/ModifyFullname";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        internal bool ModifyIdentity(MerchantProfile profile)
        {
            var url = $"{baseAddress}/ModifyIdentity";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }


        public bool Insert(MerchantProfile profile)
        {
            var url = $"{baseAddress}/AddMerchant";
            var pa = JsonConvert.SerializeObject(profile);
            _log.Info(pa);
            var result = RestUtilities.PostJson(url, headers, pa);
            _log.Info(result);

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        public bool ModifyAddress2(MerchantProfile profile)
        {
            var url = $"{baseAddress}/ModifyAddress2";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        public bool UpdateMerchantLicense(Guid merchantId, string companyName, string licenseNo, Guid businessLicense)
        {
            var url = $"{baseAddress}/UpdateMerchantLicense";
            var obj = new UpdateMerchantLicenseIM()
            {
                MerchantId = merchantId,
                CompanyName = companyName,
                LicenseNo = licenseNo,
                BusinessLicense = businessLicense
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public MerchantProfile GetById(Guid id)
        {
            var url = $"{baseAddress}/GetById";
            var obj = new
            {
                Id = id
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<MerchantProfile>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool CommitBusinessLicense(MerchantProfile profile)
        {
            var url = $"{baseAddress}/CommitBusinessLicense";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        public bool CommitIdentityImage(MerchantProfile profile)
        {
            var url = $"{baseAddress}/CommitIdentityImage";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }


        public bool UpdateAddress(Guid merchantId, string postCode, string address1, string address2)
        {
            var url = $"{baseAddress}/UpdateAddress";
            var obj = new
            {
                Id = merchantId,
                PostCode = postCode,
                Address1 = address1,
                Address2 = address2
            };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(obj));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool UpdateAddress(Address address)
        {
            var url = $"{baseAddress}/UpdateAddress";

            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(address));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        public bool UpdateCellphone(MerchantProfile profile)
        {
            var url = $"{baseAddress}/UpdateCellphone";
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(profile));

            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public List<MerchantProfile> GetListByIds(List<Guid> ids)
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

        private string GenerateToken()
        {
            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + server.ClientKey;
            string token = AES128.Encrypt(password, server.SecretKey);

            return token;
        }

        internal bool UpdateL1VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            var url = $"{baseAddress}/UpdateL1VerifyStatus";
            UpdateVerifyStatusIM im = new UpdateVerifyStatusIM() { Id = id, VerifyStatus = verifyStatus, Remark = remark };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(im));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }
        internal bool UpdateL2VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            var url = $"{baseAddress}/UpdateL2VerifyStatus";
            UpdateVerifyStatusIM im = new UpdateVerifyStatusIM() { Id = id, VerifyStatus = verifyStatus, Remark = remark };
            var result = RestUtilities.PostJson(url, headers, JsonConvert.SerializeObject(im));
            var data = JsonConvert.DeserializeObject<ServiceResult<bool>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        internal int GetCountByIdentityDocNo(string IdentityDocNo)
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
