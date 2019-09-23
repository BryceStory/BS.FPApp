using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class MerchantAccountDAC : BaseDataAccess
    {
        public void Insert(MerchantAccount model)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantAccounts] ([Id],[PhoneCode],[Cellphone],[Username],[PIN],[MerchantName],[Status],[POSId],[CountryId],[RegistrationDate],[SecretKey],[FiatCurrency], [Markup],[Receivables_Tier],[ValidationFlag],[InvitationCode])
                                      VALUES (@Id,@PhoneCode,@Cellphone,@Username,@PIN,@MerchantName,@Status,@POSId,@CountryId,@RegistrationDate,@SecretKey,@FiatCurrency,@Markup,@Receivables_Tier,@ValidationFlag,@InvitationCode);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, model);
            }
        }

            
        public MerchantAccount GetById(Guid id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT * FROM MerchantAccounts WHERE Id=@Id", new { Id = id });
            }
        }

        public MerchantAccount GetByUsername(string username)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT * FROM MerchantAccounts WHERE Username=@Username", new { Username = username });
            }
        }

        public MerchantAccount GetByCellphone(string cellphone)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT * FROM MerchantAccounts WHERE Cellphone=@Cellphone",
                new { Cellphone = cellphone });
            }
        }

        public MerchantAccount GetByEmail(string email)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT * FROM MerchantAccounts WHERE Email=@Email", new { Email = email });
            }
        }

        public bool UpdateEmail(Guid merchantId, string email)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET Email=@Email WHERE Id=@Id";
                return con.Execute(SQL, new { Email = email, Id = merchantId }) > 0;
            }
        }

        public bool UpdateL1VerfiyStatus(Guid merchantId, byte status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE dbo.MerchantAccounts SET L1VerifyStatus=@L1VerifyStatus WHERE Id=@Id";
                return con.Execute(SQL, new { L1VerifyStatus = status, Id = merchantId }) > 0;
            }
        }

        public bool UpdateL2VerfiyStatus(Guid merchantId, byte status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE dbo.MerchantAccounts SET L2VerifyStatus=@L2VerifyStatus WHERE Id=@Id";
                return con.Execute(SQL, new { L2VerifyStatus = status, Id = merchantId }) > 0;
            }
        }

        public bool UpdateGoogleAuthencator(Guid id, string secret, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET AuthSecretKey=@AuthSecretKey,ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, AuthSecretKey = secret, ValidationFlag = flag }) > 0;
            }
        }
        public bool UpdateGoogleAuthencator(Guid id, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, ValidationFlag = flag }) > 0;
            }
        }

        public bool SetAuthSecretById(Guid id, string secret)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET AuthSecretKey=@AuthSecretKey WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, AuthSecretKey = secret }) > 0;
            }
        }

        public bool UpdateCellphone(Guid merchantId, string cellphone)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET Cellphone=@Cellphone WHERE Id=@Id";
                return con.Execute(SQL, new { Cellphone = cellphone, Id = merchantId }) > 0;
            }
        }

        public void RemoveById(Guid accountId)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"DELETE FROM [dbo].[MerchantAccounts] WHERE Id=@AccountId", new { AccountId = accountId });
            }
        }

        //public bool UpdateAuthSecretKey(Guid merchantId, string secretKey)
        //{
        //    using (var con = WriteConnection())
        //    {
        //        string SQL = "UPDATE MerchantAccounts SET ValidationFlag=@ValidationFlag WHERE Id = @Id";
        //        return con.Execute(SQL, new { Id = id, ValidationFlag = flag }) > 0;
        //    }
        //}

        public bool SetValidateFlagById(Guid id, byte flag)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET ValidationFlag=@ValidationFlag WHERE Id = @Id";
                return con.Execute(SQL, new { Id = id, ValidationFlag = flag }) > 0;
            }
        }

        public MerchantAccount GetByPosSn(string possn, string username)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT a.* FROM MerchantAccounts a JOIN POS b ON a.POSId=b.Id WHERE b.SN=@SN AND a.Username=@Username",
                new { SN = possn, Username = username });
            }
        }


        public MerchantAccount GetByPosSn(string sn)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("SELECT a.* FROM MerchantAccounts a JOIN POS b ON a.POSId=b.Id WHERE b.SN=@SN", new { SN = sn });
            }
        }
        public void SettingPinById(Guid merchantAccountId, string hashPassword)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
                                    SET [PIN] = @PIN
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { Id = merchantAccountId, PIN = hashPassword });
            }
        }

        public List<UserSecretES> ListAllSecretKeys()
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT Id, SecretKey FROM MerchantAccounts";
                return con.Query<UserSecretES>(SQL).ToList();
            }
        }

        public List<MerchantAccount> GetListByIdList(List<Guid> idList)
        {
            if (idList.Count == 0)
            {
                return new List<MerchantAccount> { };
            }
            using (var con = ReadConnection())
            {
                var idListStr = string.Join(",", idList.Select(a => $"'{a}'"));
                string SQL = $"SELECT * FROM MerchantAccounts WHERE Id in ({idListStr})";
                return con.Query<MerchantAccount>(SQL).ToList();
            }
        }

        public void UpdatePIN(MerchantAccount account)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"UPDATE MerchantAccounts SET PIN=@PIN WHERE Id=@Id", account);
            }
        }


        #region Merchant_Web
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MerchantAccount GetMerchantAccountByUserName(string userName)
        {

            string strSql = @"SELECT * FROM  dbo.MerchantAccounts  WHERE Username=@Username";

            using (var con = ReadConnection())
            {
                return con.QueryFirst<MerchantAccount>(strSql, new { Username = userName });
            }
        }

        /// <summary>
        /// 获取商家基本信息
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public MerchantBaseInfo GetMerchantBaseInfo_Web(Guid merchantId)
        {

            string strSql = @"SELECT a.*,b.SN as PosSN,a.CountryId,c.InviterCode FROM dbo.MerchantAccounts a
                 Left Join dbo.POS b on a.POSId=b.Id 
                 Left Join dbo.InviteRecords c on a.Id  = c.AccountId WHERE a.Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirst<MerchantBaseInfo>(strSql, new { Id = merchantId });
            }
        }

        /// <summary>
        /// 修改商家账户[部分]
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="merchantName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public int UpdateMerchantAccount_Web(Guid merchantId, string merchantName, string email)
        {
            string strSql = @"UPDATE [dbo].[MerchantAccounts]
                                  SET [MerchantName] = @MerchantName, [Email] = @Email  WHERE Id=@MerchantId";
            using (var con = WriteConnection())
            {
                int result = con.Execute(strSql, new { MerchantId = merchantId, MerchantName = merchantName, Email = email });
                return result;
            }
        }

        /// <summary>
        /// 修改商家头像
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public bool UpdateMerchantHeadImage(Guid merchantId, string photoId)
        {
            string strSql = @"UPDATE [dbo].[MerchantAccounts]
                                  SET [Photo] = @Photo  WHERE Id=@MerchantId";
            using (var con = WriteConnection())
            {
                bool result = con.Execute(strSql, new { MerchantId = merchantId, Photo = photoId }) > 0;
                return result;
            }
        }



        public string QueryKYCServer(Guid? id)
        {
            string server = null;
            using (var con = WriteConnection())
            {
                string SQL = "select ServerAddress from ProfileRouter where Country=(select countryId from MerchantAccounts where Id=@Id)";
                server = con.ExecuteScalar<string>(SQL, new { Id = id });
            }
            return server;
        }

        public bool UpdateMerchantName(Guid? id, string merchantName)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE MerchantAccounts SET MerchantName=@MerchantName WHERE Id=@Id";
                return con.Execute(SQL, new { MerchantName = merchantName, Id = id }) > 0;
            }
        }

        #endregion

        public void BindPos(MerchantAccount account)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
                                    SET [POSId] = @POSId
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    account.Id,
                    account.POSId
                });
            }
        }

        public void UnbindingAccount(MerchantAccount account)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
                                    SET [POSId] = @POSId,
                                        [ValidationFlag] = @ValidationFlag
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    account.Id,
                    account.POSId,
                    account.ValidationFlag
                });
            }
        }

        public void SettingMarkup(Guid accountId, decimal markup)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
                                    SET [Markup] = @Markup
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = accountId,
                    Markup = markup
                });
            }
        }

        public void SettingFiatCurrency(Guid accountId, string fiatCurrency)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
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

        //public void SettingMinerCryptoAddress(Guid accountId, string address)
        //{
        //    const string sql = @"UPDATE [dbo].[MerchantAccounts]
        //                            SET [MinerCryptoAddress] = @MinerCryptoAddress
        //                          WHERE [Id] = @Id";
        //    using (var conn = WriteConnection())
        //    {
        //        conn.Execute(sql, new
        //        {
        //            Id = accountId,
        //            MinerCryptoAddress = address
        //        });
        //    }
        //}

        public void SetWithdrawalFeeType(Guid accountId, WithdrawalFeeType type)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
                                    SET [WithdrawalFeeType] = @WithdrawalFeeType
                                  WHERE [Id] = @Id";

            using (var connection = WriteConnection())
            {
                connection.Execute(sql, new { WithdrawalFeeType = type, Id = accountId });
            }
        }

        public void UpdateLanguage(Guid accountId, string language)
        {
            const string sql = @"UPDATE [dbo].[MerchantAccounts]
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

        public bool IsExist(string sn)
        {
            const string sql = @"SELECT ISNULL((SELECT TOP(1) 1 FROM [MerchantAccounts] WHERE [POSId]=(SELECT TOP(1) [Id] FROM [POS] WHERE [SN]=@sn)), 0)";
            using (var connection = WriteConnection())
            {
                int result = connection.ExecuteScalar<int>(sql, new { sn });
                return result > 0;
            }
        }

        /// <summary>
        /// 查询商家是否认证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public MerchantAccount SelectId(Guid id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantAccount>("select * from [dbo].[MerchantAccounts] WHERE L1VerifyStatus=@L1VerifyStatus AND L2VerifyStatus=@L2VerifyStatus AND Id=@id", new { Id = id , L1VerifyStatus =VerifyStatus.Certified, L2VerifyStatus =VerifyStatus.Certified});
            }
        }
    }
}
