using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPos.InviteReward.ComplementData
{
    public class TempDataDAC : FiiiPay.Data.BaseDataAccess
    {
        public List<FiiiPayRewardMessage> GetMessageList(DateTime startTime,DateTime endTime)
        {
            using (var con = ReadConnection())
            {
                return con.Query<FiiiPayRewardMessage>("SELECT * FROM [dbo].[TempData] WHERE [Status]=0").ToList();
            }
        }

        public void MessageComplated(Guid Id)
        {
            using (var con = ReadConnection())
            {
                con.Execute("UPDATE [dbo].[TempData] SET [Status]=1 WHERE Id=@Id",new { Id });
            }
        }
    }
}
