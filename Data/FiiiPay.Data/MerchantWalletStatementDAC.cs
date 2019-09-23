using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MerchantWalletStatementDAC : BaseDataAccess
    {
        public long Insert(MerchantWalletStatement merchantWalletStatement)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>("INSERT INTO MerchantWalletStatements(WalletId,Action,Amount,Balance,Timestamp,Remark) VALUES(@WalletId,@Action,@Amount,@Balance,@Timestamp,@Remark); SELECT SCOPE_IDENTITY()", merchantWalletStatement);
            }
        }
    }
}