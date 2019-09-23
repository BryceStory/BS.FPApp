using Dapper;
using FiiiPay.Entities;
using System;

namespace FiiiPay.Data
{
    public partial class MerchantProfilesDAC : BaseDataAccess
    {
        public MerchantProfile GetById(Guid id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantProfile>("SELECT * FROM MerchantProfiles WHERE MerchantId=@Id", new { Id = id });
            }
        }

        #region Merchant_Web

        /// <summary>
        /// 修改认证
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="businessLicense"></param>
        /// <returns></returns>
        public int UpdateMerchantLicense_Web(Guid merchantId, string licenseNo, string businessLicense)
        {
            string strSql = @"UPDATE [dbo].[MerchantProfiles]
                             SET [BusinessLicense] =@BusinessLicense,[LicenseNo] = @LicenseNo
                             WHERE MerchantId=@MerchantId";

            using (var con = WriteConnection())
            {
                return con.Execute(strSql, new { BusinessLicense = businessLicense, LicenseNo = licenseNo, MerchantId = merchantId });
            }
        }

        /// <summary>
        /// 修改商家信息[部分]
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="cityId"></param>
        /// <param name="postCode"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <returns></returns>
        public int UpdateMerchantProfiles_Web(Guid merchantId, int cityId, string postCode, string address1, string address2)
        {
            string strSql = @"UPDATE [dbo].[MerchantProfiles]
                              SET [Address1] = @Address1,[Address2] = @Address2,[Postcode] = @Postcode
                              WHERE MerchantId=@MerchantId";  //,[City] =@City
            using (var con = WriteConnection())
            {
                int result = con.Execute(strSql, new { MerchantId = merchantId, City = cityId, Postcode = postCode, Address1 = address1, Address2 = address2 });
                return result;
            }
        }
        #endregion
    }
}
