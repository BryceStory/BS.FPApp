using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class CryptocurrenciesBLL : BaseBLL
    {
        public SaveResult Update(Cryptocurrencies cur, List<PriceInfos> priceList, int userId, string userName)
        {
            Cryptocurrencies tmp = FoundationDB.DB.Queryable<Cryptocurrencies>().Where(t => t.Name.Equals(cur.Name)).First();
            if (tmp != null && cur.Id != tmp.Id)
            {
                return new SaveResult(false, "The name already exists.");
            }
            Cryptocurrencies oldCur = FoundationDB.CryptocurrencyDb.GetById(cur.Id);
            if (cur.IconURL != null)
                oldCur.IconURL = cur.IconURL;
            oldCur.Withdrawal_Tier = cur.Withdrawal_Tier / 100;
            oldCur.Withdrawal_Fee = cur.Withdrawal_Fee;
            oldCur.Sequence = cur.Sequence;
            oldCur.DecimalPlace = cur.DecimalPlace;
            oldCur.NeedTag = cur.NeedTag;
            oldCur.Name = cur.Name;
            oldCur.IsFixedPrice = cur.IsFixedPrice;
            oldCur.Code = cur.Code;
            oldCur.Status = cur.Status;
            oldCur.Enable = cur.Enable;
            oldCur.IsWhiteLabel = cur.IsWhiteLabel;

            StringBuilder sb = new StringBuilder();
            if (cur.IsFixedPrice)
            {
                var pl = FoundationDB.DB.Queryable<PriceInfos>().Where(t => t.CryptoID == cur.Id).ToList();
                if (pl.Count == 0)
                {
                    FoundationDB.PriceInfoDb.InsertRange(priceList.ToArray());
                }
                else
                {
                    int count = 0;
                    priceList.ForEach(data =>
                    {
                        sb.Append(" Update dbo.PriceInfo " +
                            "SET " +
                                "[Price]=@Price" + count.ToString() +
                        " WHERE [CryptoID]=@CryptoId" + count.ToString() + " and [CurrencyID] = @CurrencyID" + count.ToString() + "; ");
                        count++;
                    });
                    count = 0;
                    SugarParameter[] Paras = new SugarParameter[(priceList.Count * 3)];
                    int paraCount = 0;
                    priceList.ForEach(data =>
                    {
                        Paras[count] = new SugarParameter("@Price" + paraCount.ToString(), priceList[paraCount].Price);
                        count++;
                        Paras[count] = new SugarParameter("@CryptoID" + paraCount.ToString(), priceList[paraCount].CryptoID);
                        count++;
                        Paras[count] = new SugarParameter("@CurrencyID" + paraCount.ToString(), priceList[paraCount].CurrencyID);
                        count++;
                        paraCount++;
                    });
                    FoundationDB.DB.Ado.ExecuteCommand(sb.ToString(), Paras);
                }
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Cryptocurrencies " + cur.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FoundationDB.CryptocurrencyDb.Update(oldCur));
        }

        public SaveResult<int> Create(Cryptocurrencies cur, int userId, string userName)
        {
            Cryptocurrencies tmp = FoundationDB.DB.Queryable<Cryptocurrencies>().Where(t => t.Name.Equals(cur.Name)).First();
            if (tmp != null)
            {
                return new SaveResult<int>(false, "The name already exists.", tmp.Id);
            }
            cur.Withdrawal_Tier = cur.Withdrawal_Tier / 100;
            int newId = FoundationDB.CryptocurrencyDb.InsertReturnIdentity(cur);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Cryptocurrencies " + cur.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            return new SaveResult<int>(true, newId);
        }
    }
}