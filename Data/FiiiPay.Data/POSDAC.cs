using Dapper;
using FiiiPay.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.Data
{
    public class POSDAC : BaseDataAccess
    {
        public POS GetBySn(string sn)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<POS>("SELECT * FROM POS WHERE SN=@SN", new { SN = sn });
            }
        }

        public POS GetInactivedBySn(string sn)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<POS>("SELECT * FROM POS WHERE [Status]=@Status AND SN=@SN", new { Status = POSStatus.Inactived, SN = sn });
            }
        }

        public void ActivePOS(POS pos)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE POS SET [Status]=@Status WHERE SN=@SN", new { Status = POSStatus.Actived, SN = pos.Sn });
            }
        }

        public void InactivePOS(POS pos)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE POS SET [Status]=@Status WHERE SN=@SN", new { Status = POSStatus.Inactived, SN = pos.Sn });
            }
        }

        public POS GetById(long posId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<POS>("SELECT * FROM POS WHERE Id=@Id", new { Id = posId });
            }
        }

        public bool BatchUpdate(List<long> ids, bool enable)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("UPDATE POS SET [IsMiningEnabled]= @IsEnable WHERE Id in @Ids", new { IsEnable = enable, Ids = ids.ToArray() }) > 0;
            }
        }

        public bool BatchMarkWhiteLabel(List<long> ids, string whiteLabel, string firstCrypto)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("UPDATE POS SET [IsWhiteLabel]= @IsWhiteLabel,[WhiteLabel]=@WhiteLabel,[FirstCrypto]=@FirstCrypto WHERE Id in @Ids", new { IsWhiteLabel = true, WhiteLabel = whiteLabel, FirstCrypto = firstCrypto, Ids = ids.ToArray() }) > 0;
            }
        }

        public bool BatchUnMarkWhiteLabel(List<long> ids)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("UPDATE POS SET [IsWhiteLabel]= @IsWhiteLabel WHERE Id in @Ids", new { IsWhiteLabel = false, Ids = ids.ToArray() }) > 0;
            }
        }

        public List<POS> GetWhiteLabel()
        {
            using (var con = ReadConnection())
            {
                return con.Query<POS>("SELECT * FROM POS WHERE IsWhiteLabel = 1").ToList();
            }
        }

        public string GetWhiteLabelCryptoCode()
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<string>("SELECT FirstCrypto FROM POS WHERE IsWhiteLabel = 1");
            }
        }
    }
}