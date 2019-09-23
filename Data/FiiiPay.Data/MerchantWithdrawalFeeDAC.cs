using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MerchantWithdrawalFeeDAC : BaseDataAccess
    {
        public void Create(MerchantWithdrawalFee merchantWithdrawalFee)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantWithdrawalFee] ([WithdrawalId],[Amount],[Fee],[Timestamp],[Remark])
                                    VALUES (@WithdrawalId,@Amount,@Fee,@Timestamp,@Remark)";

            using (var conn = WriteConnection())
            {
                conn.Execute(sql, merchantWithdrawalFee);
            }
        }
    }
}