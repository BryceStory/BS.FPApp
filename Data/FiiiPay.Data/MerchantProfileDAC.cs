using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class MerchantProfileDAC : BaseDataAccess
    {
        public MerchantProfile GetById(Guid id)
        {
            using (var con = WriteConnection())
            {
                string SQL = "SELECT * FROM [MerchantProfiles] WHERE MerchantId=@Id";
                return con.QueryFirstOrDefault<MerchantProfile>(SQL, new { Id = id });
            }
        }

        public bool Insert(MerchantProfile model)
        {
            using (var con = WriteConnection())
            {
                //Address1,Postcode,City,State
                return con.Execute(@"INSERT INTO MerchantProfiles([MerchantId],[LastName],[FirstName],[L1VerifyStatus],[L2VerifyStatus],[IdentityDocNo],[IdentityDocType],[IdentityExpiryDate],[Address1],[Address2],[City],[State],[Postcode],[Country],[FrontIdentityImage],[BackIdentityImage],[HandHoldWithCard],[BusinessLicenseImage],[LicenseNo],[CompanyName],[L1Remark],[L2Remark],[Cellphone],[L1SubmissionDate],[L2SubmissionDate]) VALUES(
                                                                  @MerchantId,@LastName,@FirstName,@L1VerifyStatus,@L2VerifyStatus,@IdentityDocNo,@IdentityDocType,@IdentityExpiryDate,@Address1,@Address2,@City,@State,@Postcode,@Country,@FrontIdentityImage,@BackIdentityImage,@HandHoldWithCard,@BusinessLicenseImage,@LicenseNo,@CompanyName,@L1Remark,@L2Remark,@Cellphone,@L1SubmissionDate,@L2SubmissionDate)", model) > 0;
            }
        }
        public bool Delete(Guid merchantId)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("DELETE FROM MerchantProfiles WHERE MerchantId = @MerchantId", new
                {
                    MerchantId = merchantId
                }) > 0;
            }
        }
        public List<MerchantProfile> GetMerchantVerifyListL1(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {
            totalCount = this.GetCountOfMerchnatL1VerifyList(cellphone, countryId, status);
            string SQL = "SELECT * FROM MerchantProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone like @Cellphone ";
            }
            if (status != null)
            {
                //查询全部
                SQL += $" AND  L1VerifyStatus = {status.Value}  ";
            }
            SQL += $" ORDER BY {orderByFiled}  ";
            if (isDesc)
            {
                SQL += "DESC ";
            }
            SQL += ",MerchantId ";
            SQL += $" offset {(index - 1) * pageSize} rows fetch next {pageSize} rows only";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantProfile>(SQL, new
                {
                    Cellphone = "%" + cellphone + "%",
                    Country = countryId
                }).AsList();
            }

        }
        public List<MerchantProfile> GetMerchantVerifyListL2(string cellphone, int countryId, int? status, string orderByFiled, bool isDesc, int pageSize, int index, out int totalCount)
        {
            totalCount = this.GetCountOfMerchnatL2VerifyList(cellphone, countryId, status);

            string SQL = "SELECT * FROM MerchantProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone like @Cellphone ";
            }
            if (status != null)
            {
                //查询全部
                SQL += $" AND  L2VerifyStatus = {status.Value}  ";
            }
            SQL += $" ORDER BY {orderByFiled}  ";
            if (isDesc)
            {
                SQL += "DESC ";
            }
            SQL += ",MerchantId ";
            SQL += $" offset {(index - 1) * pageSize} rows fetch next {pageSize} rows only";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantProfile>(SQL, new
                {
                    Cellphone = "%" + cellphone + "%",
                    Country = countryId
                }).AsList();
            }
        }

        private int GetCountOfMerchnatL1VerifyList(string cellphone, int countryId, int? status)
        {
            //SELECT * FROM MerchantProfiles 
            string SQL = "SELECT count(1) FROM MerchantProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone like @Cellphone ";
            }
            if (status != null)
            {
                //查询全部
                SQL += $" AND  L1VerifyStatus = {status.Value}  ";
            }
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(SQL, new
                {
                    Cellphone = "%" + cellphone + "%",
                    Country = countryId
                });
            }
        }

        private int GetCountOfMerchnatL2VerifyList(string cellphone, int countryId, int? status)
        {
            //SELECT * FROM MerchantProfiles 
            string SQL = "SELECT count(1) FROM MerchantProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone like @Cellphone ";
            }
            if (status != null)
            {
                //查询全部
                SQL += $" AND L2VerifyStatus = {status.Value}  ";
            }
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(SQL, new
                {
                    Cellphone = "%" + cellphone + "%",
                    Country = countryId
                });
            }
        }

        public List<MerchantProfile> GetListByIds(List<Guid> guids)
        {
            string where = null;
            if (guids.Count > 0)
            {
                where = string.Format("N'{0}'", string.Join("\',\'", guids));
            }
            else
            {
                return null;
            }
            using (var con = ReadConnection())
            {
                string SQL = $"SELECT * FROM MerchantProfiles WHERE MerchantId in ({where})";
                return con.Query<MerchantProfile>(SQL).AsList();
            }
        }
        public bool ModifyAddress1(MerchantProfile profile)
        {
            using (var con = WriteConnection())
            {
                //Address1,Postcode,City,State
                string SQL = "UPDATE MerchantProfiles SET Address1=@Address1,Postcode=@Postcode,City=@City,State=@State,L2SubmissionDate=GETUTCDATE()  WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, profile) > 0;
            }
        }

        public bool ModifyFullname(MerchantProfile profile)
        {
            using (var con = WriteConnection())
            {
                //Address1,Postcode,City,State
                string SQL = "UPDATE MerchantProfiles SET LastName=@LastName,FirstName=@FirstName,L1SubmissionDate=GETUTCDATE() WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, profile) > 0;
            }
        }
        public bool ModifyIdentity(MerchantProfile profile)
        {
            using (var con = WriteConnection())
            {
                //Address1,Postcode,City,State
                string SQL = "UPDATE MerchantProfiles SET IdentityDocNo=@IdentityDocNo,L1SubmissionDate=GETUTCDATE() WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, profile) > 0;
            }
        }

        public bool UpdateLicenseInfo(MerchantLicenseInfo info)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE [MerchantProfiles] SET BusinessLicenseImage=@BusinessLicenseImage,LicenseNo=@LicenseNo WHERE MerchantId=@Id";
                return con.Execute(SQL, info) > 0;
            }

        }

        public bool CommitBusinessLicense(MerchantProfile profile)
        {
            profile.L2VerifyStatus = VerifyStatus.UnderApproval;
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE [MerchantProfiles] SET BusinessLicenseImage=@BusinessLicenseImage,LicenseNo=@LicenseNo,L2VerifyStatus=@L2VerifyStatus,CompanyName=@CompanyName WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, profile) > 0;
            }
        }

        public bool UpdateMerchantLicense(Guid merchantId, string companyName, string licenseNo, Guid businessLicense)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE [MerchantProfiles] SET BusinessLicenseImage=@BusinessLicenseImage,LicenseNo=@LicenseNo,CompanyName=@CompanyName,L2VerifyStatus=@L2VerifyStatus WHERE MerchantId=@MerchantId ";
                int r =
                    con.Execute(SQL, new
                    {
                        BusinessLicenseImage = businessLicense,
                        LicenseNo = licenseNo,
                        CompanyName = companyName,
                        MerchantId = merchantId,
                        L2VerifyStatus = VerifyStatus.UnderApproval
                    });
                return r > 0;
            }
        }

        public bool ModifyAddress2(MerchantProfile profile)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantProfiles SET Address2=@Address2 WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, new { }) > 0;
            }
        }

        public bool UpdateAddress(Guid merchantId, string postCode, string state, string city, string address1,string address2)
        {
            string SQL = "UPDATE MerchantProfiles SET Postcode=@Postcode,State=@State,City=@City,Address1=@Address1,Address2=@Address2 WHERE MerchantId=@MerchantId";
            using (var con = WriteConnection())
            {
                return con.Execute(SQL, new
                {
                    Postcode = postCode,
                    State = state,
                    City = city,
                    Address1 = address1,
                    Address2 = address2,
                    MerchantId = merchantId
                }) > 0;
            }
        }

        public bool UpdateAddress(Guid merchantId, string postCode, string state, string city, string address)
        {
            string SQL = "UPDATE MerchantProfiles SET Postcode=@Postcode,State=@State,City=@City,Address1=@Address WHERE MerchantId=@MerchantId";
            using (var con = WriteConnection())
            {
                return con.Execute(SQL, new
                {
                    Postcode = postCode,
                    State = state,
                    City = city,
                    Address = address,
                    MerchantId = merchantId
                }) > 0;
            }
        }

        public bool UpdateL1VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            string SQL = "UPDATE MerchantProfiles SET  L1VerifyStatus=@VerifyStatus,L1Remark=@Remark WHERE MerchantId=@MerchantId";
            using (var con = WriteConnection())
            {
                return con.Execute(SQL, new
                {
                    MerchantId = id,
                    VerifyStatus = verifyStatus,
                    Remark = remark
                }) > 0;
            }
        }

        public bool UpdateL2VerifyStatus(Guid id, VerifyStatus verifyStatus, string remark)
        {
            string SQL = "UPDATE MerchantProfiles SET L2VerifyStatus=@VerifyStatus,L2Remark=@Remark WHERE MerchantId=@MerchantId";
            using (var con = WriteConnection())
            {
                return con.Execute(SQL, new
                {
                    MerchantId = id,
                    VerifyStatus = verifyStatus,
                    Remark = remark
                }) > 0;
            }
        }

        public bool CommitIdentityImage(MerchantProfile profile)
        {
            profile.L1VerifyStatus = VerifyStatus.UnderApproval;
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE [MerchantProfiles] SET FirstName = @FirstName, LastName = @LastName, IdentityDocType = @IdentityDocType, IdentityDocNo = @IdentityDocNo, FrontIdentityImage = @FrontIdentityImage, BackIdentityImage = @BackIdentityImage, HandHoldWithCard = @HandHoldWithCard, L1VerifyStatus=@L1VerifyStatus,L1SubmissionDate=GETUTCDATE() WHERE MerchantId=@MerchantId";
                return con.Execute(SQL, profile) > 0;
            }
        }

        public int GetCountByIdentityDocNo(string IdentityDocNo)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<int>("SELECT count(1) FROM MerchantProfiles WHERE IdentityDocNo=@No and L1VerifyStatus = 1", new { No = IdentityDocNo });
            }
        }
    }
}
