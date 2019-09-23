using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class UserProfileDAC : BaseDataAccess
    {
        public UserProfile GetById(Guid id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserProfile>("SELECT * FROM UserProfiles WHERE UserAccountId=@Id", new { Id = id });
            }
        }

        public bool UpdateLv2Info(Lv2Info im)
        {
            string SQL = "UPDATE UserProfiles SET ResidentImage = @ResidentImage, Country = @Country, Postcode = @Postcode, State = @State, City = @City, Address2 = @Address2, Address1 = @Address1, L2VerifyStatus=@L2VerifyStatus,L2SubmissionDate=GETUTCDATE() WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, im) > 0;
            }
        }

        public bool UpdatePhoneNumber(Guid id, string cellphone)
        {
            string SQL = "UPDATE UserProfiles SET Cellphone = @Cellphone WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new {
                    Id=id,
                    Cellphone = cellphone
                }) > 0;
            }
        }

        public bool UpdateL1Status(Guid id, int verifyStatus,string remark)
        {
            string SQL = "UPDATE UserProfiles SET L1VerifyStatus = @L1VerifyStatus,L1Remark=@Remark WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new {
                    Id= id,
                    L1VerifyStatus= verifyStatus,
                    Remark= remark
                }) > 0;
            }
        }
        public bool UpdateL2Status(Guid id, int verifyStatus, string remark)
        {
            string SQL = "UPDATE UserProfiles SET L2VerifyStatus = @L2VerifyStatus,L2Remark=@Remark WHERE UserAccountId = @UserAccountId";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new
                {
                    UserAccountId = id,
                    L2VerifyStatus = verifyStatus,
                    Remark = remark

                }) > 0;
            }
        }

        public int GetCountOfUserProfileListForL1(string cellphone, int country, int? verifyStatus)
        {
            string SQL = $"SELECT count(1) FROM UserProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone=@Cellphone ";
            }
            if (verifyStatus != null)
            {
                SQL += " AND L1VerifyStatus=@L1VerifyStatus ";
            }
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(SQL, new
                {
                    Cellphone = cellphone,
                    Country = country,
                    L1VerifyStatus = verifyStatus
                });
            }
        }
        public int GetCountOfUserProfileListForL2(string cellphone, int country, int? verifyStatus)
        {
            string SQL = $"SELECT count(1) FROM UserProfiles WHERE Country=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone=@Cellphone ";
            }
            if (verifyStatus != null)
            {
                SQL += " AND L2VerifyStatus=@L2VerifyStatus ";
            }
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(SQL, new
                {
                    Cellphone = cellphone,
                    Country = country,
                    L2VerifyStatus = verifyStatus
                });
            }
        }

        public List<UserProfile> GetListByIds(List<Guid> guids)
        {
            if (guids.Count<1)
            {
                return null;
            }
            List<Guid> list = new List<Guid>();
            string where = null;           
            list.Add(Guid.NewGuid());
            if (list.Count > 0)
            {
                where = string.Format("'{0}'", string.Join("\',\'", guids));
            }
            else
            {
                return null;
            }
            using (var con = ReadConnection())
            {
                string SQL = $"SELECT * FROM UserProfiles WHERE UserAccountId in ({where})";
                return con.Query<UserProfile>(SQL).AsList();
            }
        }

        public List<UserProfile> GetUserProfileListForL1(string cellphone, int country, string orderByFiled, bool isDesc, int? l1VerifyStatus, int pageSize, int index, out int totalCount)
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            if (index < 1)
            {
                index = 1;
            }
            totalCount = GetCountOfUserProfileListForL1(cellphone, country, l1VerifyStatus);
            using (var con = ReadConnection())
            {
                string SQL = $"SELECT * FROM UserProfiles WHERE Country=@Country ";
                if (!string.IsNullOrEmpty(cellphone))
                {
                    SQL += " AND Cellphone=@Cellphone ";
                }
                if (l1VerifyStatus != null)
                {
                    SQL += " AND L1VerifyStatus=@L1VerifyStatus ";
                }
                if (isDesc)
                {
                    SQL += $" ORDER BY {orderByFiled} desc";
                }
                else
                {
                    SQL += $" ORDER BY {orderByFiled} asc";
                }
                SQL += ",UserAccountId ";
                SQL += $" offset {(index - 1) * pageSize} rows fetch next {pageSize} rows only";
                return con.Query<UserProfile>(SQL, new
                {
                    Cellphone = cellphone,
                    Country = country,
                    L1VerifyStatus = l1VerifyStatus
                }).AsList();
            }
        }
        public List<UserProfile> GetUserProfileListForL2(string cellphone, int country, string orderByFiled, bool isDesc, int? l2VerifyStatus, int pageSize, int index, out int totalCount)
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            if (index < 1)
            {
                index = 1;
            }
            totalCount = GetCountOfUserProfileListForL2(cellphone, country, l2VerifyStatus);

            using (var con = ReadConnection())
            {
                string SQL = $"SELECT * FROM UserProfiles WHERE Country=@Country ";
                if (!string.IsNullOrEmpty(cellphone))
                {
                    SQL += " AND Cellphone=@Cellphone ";
                }
                if (l2VerifyStatus != null)
                {
                    SQL += " AND L2VerifyStatus=@L2VerifyStatus ";
                }
                if (isDesc)
                {
                    SQL += $" ORDER BY {orderByFiled} desc";
                }
                else
                {
                    SQL += $" ORDER BY {orderByFiled} asc";
                }
                SQL += $" offset {(index-1)*pageSize} rows fetch next {pageSize} rows only";
                return con.Query<UserProfile>(SQL,new {
                    Cellphone= cellphone,
                    Country=country,
                    L2VerifyStatus = l2VerifyStatus
                }).AsList();
            }
        }

        public bool UpdateLv1Info(Lv1Info im)
        {
            string SQL = "UPDATE UserProfiles SET FirstName = @FirstName, LastName = @LastName, IdentityDocType = @IdentityDocType, IdentityDocNo = @IdentityDocNo, FrontIdentityImage = @FrontIdentityImage, BackIdentityImage = @BackIdentityImage, HandHoldWithCard = @HandHoldWithCard, L1VerifyStatus=@L1VerifyStatus,L1SubmissionDate=GETUTCDATE() WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, im) > 0;
            }
        }

        public bool UpdateIdImage(Guid id, Guid frontImage, Guid backImage, Guid handHoldImage)
        {
            string SQL = "UPDATE UserProfiles SET FrontIdentityImage = @FrontIdentityImage, BackIdentityImage = @BackIdentityImage, HandHoldWithCard = @HandHoldWithCard,L1VerifyStatus=@L1VerifyStatus,L1SubmissionDate=GETUTCDATE() WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new
                {
                    Id=id,
                    FrontIdentityImage = frontImage,
                    BackIdentityImage = backImage,
                    HandHoldWithCard = handHoldImage,
                    L1VerifyStatus=VerifyStatus.UnderApproval
                }) > 0;
            }
        }

        public bool SetGenderById(Guid id, int type)
        {
            string SQL = "UPDATE UserProfiles SET Gender = @Gender WHERE UserAccountId = @Id";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new { Gender = type, Id = id }) > 0;
            }
        }

        public UserVerifiedStatus GetVerifiedStatus(Guid id)
        {
            string SQL = "SELECT IsVerifiedL1 ,IsVerifiedL1 FROM UserProfiles WHERE UserAccountId=@Id";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserVerifiedStatus>(SQL, new { Id = id });
            }
        }

        public bool AddUser(UserRegInfo model)
        {
            string SQL = "INSERT INTO UserProfiles(UserAccountId, LastName,FirstName,IdentityDocNo,IdentityDocType,Postcode,Country) VALUES(@UserAccountId, @LastName,@FirstName,@IdentityDocNo,@IdentityDocType,@Postcode,@Country)";

            using (var con = WriteConnection())
            {
                int r = con.Execute(SQL, model);
                return r > 0;
            }
        }

        public bool UpdateBirthday(Guid id, DateTime date)
        {
            string SQL = "UPDATE UserProfiles SET DateOfBirth = @DateOfBirth WHERE UserAccountId = @UserAccountId";
            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new { DateOfBirth = date, UserAccountId = id }) > 0;
            }
        }

        public bool AddProfile(UserProfile userProfile)
        {
            string SQL = "INSERT INTO UserProfiles(UserAccountId, LastName,FirstName,IdentityDocNo,IdentityDocType,Postcode,Country,Cellphone,L1VerifyStatus,L2VerifyStatus) VALUES(@UserAccountId, @LastName,@FirstName,@IdentityDocNo,@IdentityDocType,@Postcode,@Country,@Cellphone,@L1VerifyStatus,@L2VerifyStatus)";

            using (var con = WriteConnection())
            {
                int r = con.Execute(SQL, userProfile);
                return r > 0;
            }
        }

        public bool Delete(Guid? userAccountId)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("DELETE FROM UserProfiles WHERE UserAccountId = @UserAccountId", new
                {
                    UserAccountId = userAccountId
                }) > 0;
            }
        }

        public int GetCountByIdentityDocNo(string IdentityDocNo)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<int>("SELECT count(1) FROM UserProfiles WHERE IdentityDocNo=@No and L1VerifyStatus = 1", new { No = IdentityDocNo });
            }
        }

    }
}
