using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;

namespace FiiiPay.Data
{
    public class MerchantWalletDAC : BaseDataAccess
    {
        public MerchantWallet Insert(MerchantWallet wallet)
        {
            const string sql =
             @"INSERT INTO [dbo].[MerchantWallets]([MerchantAccountId],[CryptoId],[CryptoCode],[Status],[Balance],[FrozenBalance],[Address],[SupportReceipt],[Sequence])
                    VALUES (@MerchantAccountId, @CryptoId,@CryptoCode, @Status, @Balance, @FrozenBalance, @Address,@SupportReceipt,@Sequence); SELECT SCOPE_IDENTITY()";

            using (var con = WriteConnection())
            {
                wallet.Id = con.ExecuteScalar<long>(sql, wallet);
                return wallet;
            }
        }
        public MerchantWallet GetById(long id)
        {
            const string sql = "SELECT * FROM [dbo].[MerchantWallets] WHERE [Id]=@Id";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantWallet>(sql, new { Id = id });
            }
        }
        public List<MerchantWallet> GetByAccountId(Guid accountId)
        {
            const string sql = "SELECT * FROM [dbo].[MerchantWallets] WHERE [MerchantAccountId]=@MerchantAccountId";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantWallet>(sql, new { MerchantAccountId = accountId }).ToList();
            }
        }

        public List<MerchantWallet> SupportReceiptList(Guid accountId)
        {
            const string sql = "SELECT * FROM [dbo].[MerchantWallets] WHERE MerchantAccountId=@MerchantAccountId AND SupportReceipt=1";
            using (var conn = ReadConnection())
            {
                return conn.Query<MerchantWallet>(sql, new { MerchantAccountId = accountId }).ToList();
            }
        }

        public MerchantWallet GetByAccountId(Guid accountId, int cryptoId)
        {
            const string sql = "SELECT * FROM [dbo].[MerchantWallets] WHERE [MerchantAccountId]=@MerchantAccountId AND [CryptoId]=@CryptoId";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantWallet>(sql, new { MerchantAccountId = accountId, CryptoId = cryptoId });
            }
        }

        public void Increase(Guid merchantAccountId, int cryptoId, decimal amount)
        {
            const string sql = @"UPDATE [dbo].[MerchantWallets]
                                    SET [Balance]=[Balance]+@Amount
                                  WHERE [MerchantAccountId]=@MerchantAccountId
                                    AND [CryptoId]=@CryptoId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { MerchantAccountId = merchantAccountId, CryptoId = cryptoId, Amount = amount });
            }
        }

        public void Increase(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE MerchantWallets SET Balance = Balance + @amount WHERE Id = @id",
                    new { id, amount });
            }
        }

        public void Decrease(Guid merchantAccountId, int cryptoId, decimal amount)
        {
            const string sql = @"UPDATE [dbo].[MerchantWallets]
                                    SET [Balance]=[Balance]-@Amount
                                    WHERE [MerchantAccountId]=@MerchantAccountId
                                    AND [CryptoId]=@CryptoId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { MerchantAccountId = merchantAccountId, CryptoId = cryptoId, Amount = amount });
            }
        }

        public void Decrease(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE MerchantWallets SET Balance = Balance - @amount WHERE Id = @id",
                    new { id, amount });
            }
        }

        public void Freeze(long id, decimal amount)
        {
            const string sql = "UPDATE MerchantWallets SET Balance = Balance - @amount,FrozenBalance=FrozenBalance+@amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public void Unfreeze(long id, decimal amount)
        {
            const string sql = "UPDATE MerchantWallets SET Balance = Balance+@amount,FrozenBalance=FrozenBalance-@amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public void Unfreeze(long id, decimal amount, decimal frozen)
        {
            const string sql = "UPDATE MerchantWallets SET Balance = Balance+@amount,FrozenBalance=FrozenBalance-@frozen WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount, frozen });
            }
        }

        public void DecreaseFreeze(long id, decimal amount)
        {
            const string sql = "UPDATE MerchantWallets SET FrozenBalance = FrozenBalance - @amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public void Support(Guid merchantAccountId, int cryptoId, int seq)
        {
            const string sql = @"UPDATE [dbo].[MerchantWallets]
                                    SET [SupportReceipt] = 1,
                                        [Sequence]=@Sequence
                                  WHERE [MerchantAccountId]=@MerchantAccountId
                                    AND [CryptoId]=@CryptoId";
            using (var con = WriteConnection())
            {
                con.Execute(sql,
                    new
                    {
                        MerchantAccountId = merchantAccountId,
                        CryptoId = cryptoId,
                        Sequence = seq
                    });
            }
        }

        public void Reject(Guid merchantAccountId, int cryptoId, int seq = 0)
        {
            const string sql = @"UPDATE [dbo].[MerchantWallets]
                                    SET [SupportReceipt] = 0,
                                        [Sequence]=@Sequence
                                  WHERE [MerchantAccountId]=@MerchantAccountId
                                    AND [CryptoId]=@CryptoId";
            using (var con = WriteConnection())
            {
                con.Execute(sql,
                new
                {
                    MerchantAccountId = merchantAccountId,
                    CryptoId = cryptoId,
                    Sequence = seq
                });
            }
        }

        public List<MerchantTransferES> GetMerchantTransferStatementById(Guid merchantAccountId, long id, int pageIndex, int pageSize)
        {
            const string dataSql = @"SELECT a.*,b.CryptoId
                                       FROM (SELECT a.[Id], a.[MerchantWalletId], (a.[Amount]-ISNULL(b.[Fee],0)) [Amount], a.[Status], a.[Timestamp], 2 AS TransferType
                                               FROM [dbo].[MerchantWithdrawals] a
											   LEFT JOIN [dbo].[MerchantWithdrawalFee] b ON a.id=b.WithdrawalId
                                              WHERE [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId
                                          UNION ALL
                                             SELECT [Id], [MerchantWalletId], [Amount], [Status], [Timestamp], 1 AS TransferType
                                               FROM [dbo].[MerchantDeposits] 
                                              WHERE [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId
                                            ) a
							           JOIN MerchantWallets b ON a.MerchantWalletId=b.Id
								   ORDER BY [Timestamp] DESC 
									 OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                List<MerchantTransferES> data = con.Query<MerchantTransferES>(dataSql, new
                {
                    MerchantAccountId = merchantAccountId,
                    MerchantWalletId = id,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();
                return data;
            }
        }

        public void UploadAddress(long id, string walletAddress, string tag)
        {
            const string sql = @"UPDATE [dbo].[MerchantWallets]
                                    SET [Address] = @Address,[Tag]=@Tag
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql,
                    new
                    {
                        Address = walletAddress,
                        Tag = tag,
                        Id = id
                    });
            }
        }


        public MerchantWallet GetByAddressAndCrypto(int cryptoId, string address, string tag)
        {
            using (var con = ReadConnection())
            {
                if (string.IsNullOrEmpty(tag))
                    return con.QueryFirstOrDefault<MerchantWallet>("SELECT * FROM MerchantWallets WHERE CryptoId=@CryptoId and Address = @Address and Tag IS NULL", new { CryptoId = cryptoId, Address = address });
                else
                    return con.QueryFirstOrDefault<MerchantWallet>("SELECT * FROM MerchantWallets WHERE CryptoId=@CryptoId and Address = @Address and Tag=@Tag", new { CryptoId = cryptoId, Address = address, Tag = tag });
            }
        }

        public MerchantWallet GetByAddress(string Address, string Tag)
        {
            using (var con = ReadConnection())
            {
                if (string.IsNullOrEmpty(Tag))
                    return con.QueryFirstOrDefault<MerchantWallet>("SELECT * FROM MerchantWallets WHERE Address = @Address and Tag IS NULL", new { Address });
                else
                    return con.QueryFirstOrDefault<MerchantWallet>("SELECT * FROM MerchantWallets WHERE Address = @Address and Tag=@Tag", new { Address, Tag });
            }
        }

        public bool IsMerchantWalletAddress(string address)
        {
            int count = 0;
            using (var con = ReadConnection())
            {
                count = con.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM MerchantWallets WHERE Address = @Address", new { Address = address });
            }
            return count > 0;
        }
    }
}