using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using System;
using FiiiPay.Framework;
using FiiiPay.Data;
using System.Transactions;
using FiiiPay.Entities;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.Cache;
using FiiiPay.Data.Agents.JPush.Model;
using FiiiPay.Data.Agents;
using FiiiPay.Framework.Component.Verification;

namespace FiiiPay.BackOffice.BLL
{
    public class MerchantBLL : BaseBLL
    {

        public MerchantAccount GetById(Guid id)
        {
            return FiiiPayDB.MerchantAccountDb.GetById(id);
        }

        public SaveResult Unbind(Guid id, int userId, string userName)
        {
            var merchant = FiiiPayDB.MerchantAccountDb.GetById(id);
            var pos = FiiiPayDB.POSDb.GetById(merchant.POSId);
            merchant.POSId = null;
            pos.Status = false;

            using (var scope = new TransactionScope())
            {
                FiiiPayDB.MerchantAccountDb.Update(merchant);
                FiiiPayDB.POSDb.Update(pos);
                //new POSDAC().InactivePOS(pos);
                new POSMerchantBindRecordDAC().UnbindRecord(merchant.Id, pos.Id);
                if (!string.IsNullOrEmpty(merchant.InvitationCode))
                    new InviteRecordDAC().UpdateAccountInfo(Guid.Empty, Guid.Empty, null, pos.Sn);

                scope.Complete();
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantBLL).FullName + ".Unbind";
            actionLog.Username = userName;
            actionLog.LogContent = "Unbind POS,Merchant:" + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            RemoveRegInfoByUserId(id);
            return new SaveResult(true);
        }

        private void RemoveRegInfoByUserId(Guid userId)
        {
            string redisKey = $"{RedisKeys.FiiiPOS_APP_Notice_MerchantId}:{userId}";
            RedisHelper.KeyDelete(redisKey);
        }

        public SaveResult Rebind(Guid id, string POSSN, int userId, string userName)
        {
            var pos = FiiiPayDB.DB.Queryable<POSs>().Where(t => t.Sn == POSSN && t.Status == false).First();
            if (pos != null)
            {
                var merchant = FiiiPayDB.MerchantAccountDb.GetById(id);
                merchant.POSId = pos.Id;
                pos.Status = true;

                POSMerchantBindRecord posBindRecord = new POSMerchantBindRecord
                {
                    POSId = pos.Id,
                    SN = pos.Sn,
                    MerchantId = merchant.Id,
                    MerchantUsername = merchant.Username,
                    BindTime = DateTime.UtcNow,
                    BindStatus = (byte)POSBindStatus.Binded
                };
                new POSMerchantBindRecordDAC().Insert(posBindRecord);

                if (!string.IsNullOrEmpty(merchant.InvitationCode))
                {
                    UserAccount userAccount = FiiiPayDB.UserAccountDb.GetSingle(t => t.InvitationCode == merchant.InvitationCode);
                    if (userAccount != null)
                        new InviteRecordDAC().UpdateAccountInfo(userAccount.Id, merchant.Id, merchant.InvitationCode, pos.Sn);
                }

                FiiiPayDB.POSDb.Update(pos);
                FiiiPayDB.MerchantAccountDb.Update(merchant);

                // Create ActionLog
                ActionLog actionLog = new ActionLog();
                actionLog.IPAddress = GetClientIPAddress();
                actionLog.AccountId = userId;
                actionLog.CreateTime = DateTime.UtcNow;
                actionLog.ModuleCode = typeof(MerchantBLL).FullName + ".Rebind";
                actionLog.Username = userName;
                actionLog.LogContent = "Rebind Merchant:" + id + ",POSSN:" + POSSN;
                ActionLogBLL ab = new ActionLogBLL();
                ab.Create(actionLog);

                return new SaveResult(true);
            }
            else
            {
                return new SaveResult(false, "The SN does not exist or has been used");
            }
        }

        public SaveResult ChangeCellphone(int adminId, string adminName, Guid merchantId, int countryId, string cellphone)
        {
            var merchant = FiiiPayDB.MerchantAccountDb.GetById(merchantId);
            if (merchant == null)
                return new SaveResult(false);

            var country = FoundationDB.CountryDb.GetById(countryId);

            var sr = FiiiPayDB.DB.Updateable<MerchantAccounts>().SetColumns(t => new MerchantAccounts
            {
                CountryId = countryId,
                PhoneCode = country.PhoneCode,
                Cellphone = cellphone
            }).Where(t => t.Id == merchant.Id).ExecuteCommand();

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = adminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantBLL).FullName + ".ChangeCellphone";
            actionLog.Username = adminName;
            actionLog.LogContent = $"Change Cellphone from {merchant.PhoneCode + merchant.Cellphone} to {country.PhoneCode + cellphone}";
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult UpdatePIN(MerchantAccounts account, int userId, string userName)
        {
            MerchantAccounts oldMerchant = FiiiPayDB.MerchantAccountDb.GetById(account.Id);
            oldMerchant.PIN = account.PIN;

            var securityVerify = new SecurityVerification(SystemPlatform.FiiiPOS);
            securityVerify.DeleteErrorCount(SecurityMethod.Pin, account.Id.ToString());

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantBLL).FullName + ".UpdatePIN";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Merchant PIN " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.MerchantAccountDb.Update(oldMerchant));
        }
        public SaveResult DeleteErrorCount(Guid id, string type, int userId, string userName)
        {
            if (type.Equals("PIN"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new PinVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
            if (type.Equals("SMS"))
            {
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new CellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new RegisterCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new LoginCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ForgetPasswordCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdatePinCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ResetPinCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdatePasswordCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdateCellphoneOldVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdateCellphoneNewVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new MandatoryCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new BindAccountCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ModifyCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));
            }
            if (type.Equals("GoogleAuth"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new GoogleVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));

            if (type.Equals("Identity"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new MandatoryVerifier().GetErrorCountKey(SystemPlatform.FiiiPOS, id.ToString()));


            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantBLL).FullName + ".DeleteErrorCount";
            actionLog.Username = userName;
            actionLog.LogContent = "DeleteErrorCount " + id + ":" + type;
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(true, "Save Success");
        }
    }
}