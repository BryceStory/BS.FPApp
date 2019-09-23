using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiPay.BackOffice.BLL
{
    public class CurrencyBLL : BaseBLL
    {
        public SaveResult<int> Create(Currency cur, List<PriceInfos> list, int userId, string userName)
        {
            Currency tmp = FoundationDB.DB.Queryable<Currency>().Where(t => t.Name.Equals(cur.Name)).First();
            if (tmp != null)
            {
                return new SaveResult<int>(false, "The name already exists.", tmp.ID);
            }

            if (cur.IsFixedPrice)
            {
                FoundationDB.PriceInfoDb.InsertRange(list.ToArray());
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Currency " + cur.ID;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            return new SaveResult<int>(true, FoundationDB.CurrencyDb.InsertReturnIdentity(cur));
        }

        public SaveResult Update(Currency cur, List<PriceInfos> priceList, int userId, string userName)
        {
            Currency tmp = FoundationDB.DB.Queryable<Currency>().Where(t => t.Name.Equals(cur.Name)).First();
            if (tmp != null && cur.ID != tmp.ID)
            {
                return new SaveResult(false, "The name already exists.");
            }
            Currency oldCur = FoundationDB.CurrencyDb.GetById(cur.ID);

            oldCur.Name = cur.Name;
            oldCur.IsFixedPrice = cur.IsFixedPrice;
            oldCur.Code = cur.Code;
            oldCur.Name_CN = cur.Name_CN;

            StringBuilder sb = new StringBuilder();
            if (cur.IsFixedPrice)
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

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Currency " + cur.ID;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FoundationDB.CurrencyDb.Update(oldCur));
        }
    }
}