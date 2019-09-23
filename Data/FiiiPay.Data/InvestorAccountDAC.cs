using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class InvestorAccountDAC : BaseDataAccess
    {
        public InvestorAccount GetById(long accountId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<InvestorAccount>("SELECT * FROM InvestorAccounts WHERE Id=@Id", new { Id = accountId });
            }
        }

        public void Decrease(long id, decimal amount)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE InvestorAccounts SET Balance = Balance - @amount WHERE Id = @id",
                    new { id, amount });
            }
        }
        public InvestorAccount GetByUsername(string username)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<InvestorAccount>("SELECT * FROM InvestorAccounts WHERE Username=@Username", new { Username = username });
            }
        }
        
        public InvestorAccount GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<InvestorAccount>("SELECT * FROM InvestorAccounts WHERE Id=@Id", new { Id = id });
            }
        }

        public bool UpdatePassword(int id, string newPassword)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE InvestorAccounts SET Password=@Password WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, Password = newPassword }) > 0;
            }
        }

        public bool UpdatePIN(int id, string newPIN)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE InvestorAccounts SET PIN=@PIN WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, PIN = newPIN }) > 0;
            }
        }

        public bool UpdatePasswordStatus(int id, int status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE InvestorAccounts SET IsUpdatePassword=@Status WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, Status = status }) > 0;
            }
        }

        public bool UpdatePINStatus(int id,int status)
        {
            using (var con = WriteConnection())
            {
                string SQL = "UPDATE InvestorAccounts SET IsUpdatePIN=@Status WHERE Id=@Id";
                return con.Execute(SQL, new { Id = id, Status = status }) > 0;
            }
        }

    }
}
