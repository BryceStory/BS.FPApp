using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;

namespace FiiiPay.Data
{
    public class UserAccountDAC : BaseDataAccess
    {
        public void Insert(UserAccount model)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"INSERT INTO UserAccounts(
Id, Cellphone, Email, IsVerifiedEmail, RegistrationDate,PhoneCode, CountryId,Nickname,
Photo, Password, PIN, SecretKey, Status, IsAllowWithdrawal, IsAllowExpense, FiatCurrency,InvitationCode, InviterCode, ValidationFlag
) VALUES(
@Id, @Cellphone, @Email, @IsVerifiedEmail, @RegistrationDate,@PhoneCode, @CountryId,@Nickname,
@Photo, @Password, @PIN, @SecretKey, @Status, @IsAllowWithdrawal, @IsAllowExpense, @FiatCurrency,@InvitationCode, @InviterCode,@ValidationFlag
);", model);
            }
        }

        public bool UpdatePhoneNumber(Guid id, string cellphone)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Cellphone=@Cellphone WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, Cellphone = cellphone }) > 0;
            }
        }

        public bool UpdateNickname(Guid id, string name)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Nickname=@Nickname WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, Nickname = name }) > 0;
            }
        }

        public List<UserAccount> GetUserAccountStatusList(string cellphone, int country, int? status, int pageSize, int index, out int totalCount)
        {
            totalCount = this.GetCountOfUserAccountStatusList(cellphone, country, status);
            string SQL = $"SELECT * FROM UserAccounts WHERE CountryId=@CountryId ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone=@Cellphone ";
            }
            if (status.HasValue)
            {
                SQL += " AND Status = @Status";
            }
            SQL += $" ORDER BY RegistrationDate desc ";
            SQL += $" offset {(index - 1) * pageSize} rows fetch next {pageSize} rows only";
            using (var con = ReadConnection())
            {
                return con.Query<UserAccount>(SQL, new
                {
                    Cellphone = cellphone,
                    CountryId = country,
                    Status = status
                }).AsList();
            }
        }

        private int GetCountOfUserAccountStatusList(string cellphone, int country, int? status)
        {
            string SQL = $"SELECT count(*) FROM UserAccounts WHERE CountryId=@Country ";
            if (!string.IsNullOrEmpty(cellphone))
            {
                SQL += " AND Cellphone=@Cellphone ";
            }
            if (status.HasValue)
            {
                SQL += " AND Status=@Status ";
            }
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(SQL, new
                {
                    Cellphone = cellphone,
                    Country = country,
                    Status = status
                });
            }
        }

        public void Update(UserAccount model)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"UPDATE  UserAccounts SET 
Cellphone=@Cellphone,Email=@Email,IsVerifiedEmail=@IsVerifiedEmail,RegistrationDate=@RegistrationDate,CountryId=@CountryId,Photo=@Photo,
Password=@Password,PIN=@PIN,SecretKey=@SecretKey,Status=@Status,IsAllowWithdrawal=@IsAllowWithdrawal,IsAllowExpense=@IsAllowExpense,FiatCurrency=@FiatCurrency
where Id=@Id", model);
            }
        }

        public bool UpdateL1VerfiyStatus(Guid accountId, byte status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE dbo.UserAccounts SET L1VerifyStatus=@L1VerifyStatus WHERE Id=@Id";
                return con.Execute(SQL, new { L1VerifyStatus = status, Id = accountId }) > 0;
            }
        }

        public bool UpdateL2VerfiyStatus(Guid accountId, byte status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE dbo.UserAccounts SET L2VerifyStatus=@L2VerifyStatus WHERE Id=@Id";
                return con.Execute(SQL, new { L2VerifyStatus = status, Id = accountId }) > 0;
            }
        }

        public void RemoveById(Guid accountId)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"DELETE FROM [dbo].[UserAccounts] WHERE Id=@AccountId", new { AccountId = accountId });
            }
        }

        public UserAccount GetById(Guid id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserAccount>("SELECT * FROM UserAccounts WHERE Id=@Id", new { Id = id });
            }
        }

        public UserAccount GetByCountryIdAndCellphone(int countryId, string cellphone)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserAccount>("SELECT * FROM UserAccounts WHERE CountryId=@CountryId and Cellphone=@Cellphone", new { CountryId = countryId, Cellphone = cellphone });
            }
        }

        public UserAccount GetByFullPhoneCode(string phoneCode,string cellphone)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserAccount>("SELECT * FROM UserAccounts WHERE PhoneCode=@PhoneCode and Cellphone=@Cellphone", new { PhoneCode = phoneCode, Cellphone = cellphone });
            }
        }

        public UserAccount GetByInvitationCode(string code)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserAccount>("SELECT * FROM UserAccounts WHERE InvitationCode=@Code", new { Code = code });
            }
        }

        //public bool SetCellphoneById(Guid id, string cellphone)
        //{
        //    using (var con = WriteConnection())
        //    {
        //        string SQL = "UPDATE UserAccounts SET Cellphone=@Cellphone WHERE Id=@Id";
        //        return con.Execute(SQL, new {Id = id, Cellphone = cellphone }) > 0;
        //    }
        //}

        public bool SetEmailById(Guid id, string email)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Email=@Email WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, Email = email }) > 0;
            }
        }

        public bool SetPinById(Guid id, string pin)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Pin=@Pin WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, Pin = pin }) > 0;
            }
        }

        public bool SetPasswordById(Guid id, string password)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Password=@Password WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, Password = password }) > 0;
            }
        }

        public bool SetGenderById(Guid id, int type)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET Gender=@Gender WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, Gender = type }) > 0;
            }
        }

        public bool UpdateGoogleAuthencator(Guid id, string secret, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET AuthSecretKey=@AuthSecretKey,ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, AuthSecretKey = secret, ValidationFlag = flag }) > 0;
            }
        }
        public bool UpdateGoogleAuthencator(Guid id, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, ValidationFlag = flag }) > 0;
            }
        }

        public bool SetAuthSecretById(Guid id, string secret)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET AuthSecretKey=@AuthSecretKey WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, AuthSecretKey = secret }) > 0;
            }
        }

        public bool SetValidateFlagById(Guid id, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE UserAccounts SET ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, ValidationFlag = flag }) > 0;
            }
        }

        public List<UserSecretES> ListAllSecretKeys()
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT Id, SecretKey FROM UserAccounts";
                return con.Query<UserSecretES>(SQL).ToList();
            }
        }

        public UserAccount GetByEmail(string email)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserAccount>("SELECT * FROM UserAccounts WHERE Email=@Email", new { Email = email });
            }
        }

        public bool ExistInviterCode(string imInviterCode)
        {
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<int>("SELECT ISNULL((SELECT TOP(1) 1 FROM UserAccounts WHERE [InvitationCode]=@InvitationCode), 0)",
                           new { InvitationCode = imInviterCode }) > 0;
            }
        }

        public bool ExistNickname(string nickname)
        {
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<int>("SELECT ISNULL((SELECT TOP(1) 1 FROM UserAccounts WHERE [Nickname]=@Nickname), 0)",
                           new { Nickname = nickname }) > 0;
            }
        }

        public UserAccount GetUserAccountByInviteCode(string inviteCode)
        {
            var sql = $"SELECT * FROM UserAccounts WHERE InvitationCode=@InvitationCode";
            using (var conn = ReadConnection())
            {
                return conn.Query<UserAccount>(sql,
                           new { InvitationCode = inviteCode }).FirstOrDefault();
            }
        }

        public void SettingFiatCurrency(Guid accountId, string fiatCurrency)
        {
            const string sql = @"UPDATE [dbo].[UserAccounts]
                                    SET [FiatCurrency] = @FiatCurrency
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = accountId,
                    FiatCurrency = fiatCurrency
                });
            }
        }

        public void UpdateLanguage(Guid accountId, string language)
        {
            const string sql = @"UPDATE [dbo].[UserAccounts]
                                    SET [Language] = @Language
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = accountId,
                    Language = language
                });
            }
        }

        public void UpdateIsBindingDevice(Guid accountId)
        {
            const string sql = @"UPDATE [dbo].[UserAccounts]
                                    SET [IsBindingDevice] = 1
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = accountId
                });
            }
        }
    }
}
