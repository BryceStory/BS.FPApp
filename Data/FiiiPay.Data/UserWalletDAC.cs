using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserWalletDAC : BaseDataAccess
    {
        public UserWallet GetById(long id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE Id = @Id",
                    new { Id = id });
            }
        }

        public UserWallet GetByAccountId(Guid userId, int cryptoId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE UserAccountId = @UserAccountId AND CryptoId = @CryptoId",
                    new { UserAccountId = userId, CryptoId = cryptoId });
            }
        }

        public UserWallet GetByCryptoCode(Guid userId, string cryptoCode)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE UserAccountId = @UserAccountId AND CryptoCode = @CryptoCode",
                    new { UserAccountId = userId, CryptoCode = cryptoCode });
            }
        }

        public void Increase(Guid userId, int cryptoIdValue, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET Balance = Balance + @amount WHERE UserAccountId = @UserAccountId AND CryptoId = @CryptoId",
                    new { UserAccountId = userId, CryptoId = cryptoIdValue, amount });
            }
        }

        public void Increase(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET Balance = Balance + @amount WHERE Id = @id",
                    new { id, amount });
            }
        }

        public void IncreaseFrozen(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET FrozenBalance = FrozenBalance + @amount WHERE Id = @id",
                    new { id, amount });
            }
        }

        public void Decrease(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET Balance = Balance - @amount WHERE Id = @id",
                    new { id, amount });
            }
        }

        public void Freeze(long id, decimal amount)
        {
            const string sql = "UPDATE UserWallets SET Balance = Balance - @amount,FrozenBalance=FrozenBalance+@amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public void Unfreeze(long id, decimal amount)
        {
            const string sql = "UPDATE UserWallets SET Balance = Balance+@amount,FrozenBalance=FrozenBalance-@amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public void Unfreeze(long id, decimal amount, decimal frozen)
        {
            const string sql = "UPDATE UserWallets SET Balance = Balance+@amount,FrozenBalance=FrozenBalance-@frozen WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount, frozen });
            }
        }

        public void DecreaseFreeze(long id, decimal amount)
        {
            const string sql = "UPDATE UserWallets SET FrozenBalance=FrozenBalance-@amount WHERE Id = @id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { id, amount });
            }
        }

        public UserWallet GetUserWallet(Guid userId, int coinId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE UserAccountId = @UserAccountId AND CryptoId=@CryptoId", new { UserAccountId = userId, CryptoId = coinId });
            }
        }

        public List<UserWallet> GetUserWallets(Guid userId)
        {
            using (var con = ReadConnection())
            {
                return con.Query<UserWallet>("SELECT * FROM UserWallets WHERE UserAccountId = @UserAccountId", new { UserAccountId = userId }).ToList();
            }
        }

        public long Insert(UserWallet model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO UserWallets(UserAccountId,CryptoId,Balance,FrozenBalance,Address,Tag,ShowInHomePage,HomePageRank,PayRank,CryptoCode) VALUES(@UserAccountId,@CryptoId,@Balance,@FrozenBalance,@Address,@Tag,@ShowInHomePage,@HomePageRank,@PayRank,@CryptoCode); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public void ReOrderForPay(Guid id, List<int> idList)
        {
            throw new NotImplementedException();
        }

        public void ReOrderForHomePage(Guid id, List<int> idList)
        {
            throw new NotImplementedException();
        }

        public void UpdateShowInHomePage(long id, bool ShowInHomePage)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET ShowInHomePage = @ShowInHomePage WHERE Id = @id",
                    new { id, ShowInHomePage });
            }
        }

        public void UploadAddress(long id, string Address, string Tag)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE UserWallets SET Address = @Address,Tag=@Tag WHERE Id = @id",
                    new { id, Address, Tag });
            }
        }

        public bool ExistsByUserAccountIdAndAddress(string Address, Guid UserAccountId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE Address = @Address AND UserAccountId = @UserAccountId", new { Address, UserAccountId }) != null;
            }
        }

        public UserWallet GetByAddressAndCrypto(int cryptoId, string address, string tag)
        {
            using (var con = ReadConnection())
            {
                if (string.IsNullOrEmpty(tag))
                    return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE CryptoId=@CryptoId and Address = @Address and Tag IS NULL", new { CryptoId = cryptoId, Address = address });
                else
                    return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE CryptoId=@CryptoId and Address = @Address and Tag=@Tag", new { CryptoId = cryptoId, Address = address, Tag = tag });
            }
        }

        //public UserWallet GetByAddress(string address, string tag)
        //{
        //    using (var con = ReadConnection())
        //    {
        //        if (string.IsNullOrEmpty(tag))
        //            return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE Address = @Address and Tag IS NULL", new { Address = address });
        //        return con.QueryFirstOrDefault<UserWallet>("SELECT * FROM UserWallets WHERE Address = @Address and Tag=@Tag", new { Address = address, Tag = tag });
        //    }
        //}

        public bool IsUserWalletAddress(string address)
        {
            int count = 0;
            using (var con = ReadConnection())
            {
                count = con.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM UserWallets WHERE Address = @Address", new { Address = address });
            }
            return count > 0;
        }
    }
}