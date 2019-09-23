using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO.Deposit;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Business
{
    public class UserDepositComponent : BaseComponent
    {
        public DetailOM Detail(UserAccount user, long id, bool isZH)
        {
            var data = new UserDepositDAC().GetById(user.Id, id);
            if (data == null || data.UserAccountId != user.Id) throw new SystemErrorException();

            var om = new DetailOM
            {
                CryptoAmount = data.Amount.ToString(),
                Code = data.CryptoCode,
                Id = data.Id,
                OrderNo = data.OrderNo,
                StatusStr = new UserStatementComponent().GetStatusStr(0, (int)data.Status, isZH),
                Status = data.Status,
                Timestamp = data.Timestamp.ToUtcTimeTicks().ToString(),
                Type = Resources.Deposit,
                SelfPlatform = data.SelfPlatform,
                TransactionId = data.SelfPlatform ? null : data.TransactionId,
            };

            bool showCheckTime = data.CryptoCode != "XRP";

            if (showCheckTime && !data.SelfPlatform &&
                om.Status == TransactionStatus.Pending &&
                data.RequestId.HasValue)
            {
                var agent = new FiiiFinanceAgent();
                var statusInfo = agent.GetDepositStatus(data.RequestId.Value);
                om.CheckTime = $"{statusInfo.TotalConfirmation}/{statusInfo.MinRequiredConfirmation}";
            }
            return om;
        }

        public PreDepositOM PreDeposit(UserAccount user, int cryptoId)
        {
            var dac = new UserWalletDAC();
            var wallet = dac.GetByAccountId(user.Id, cryptoId) ?? new UserWalletComponent().GenerateWallet(user.Id, cryptoId);
            var coin = new CryptocurrencyDAC().GetById(cryptoId);

            if (string.IsNullOrEmpty(wallet.Address))
            {
                var agent = new FiiiFinanceAgent();
                var addressInfo = agent.CreateWallet(coin.Code, user.Id, AccountTypeEnum.User, user.Email, user.Cellphone);
                if (string.IsNullOrWhiteSpace(addressInfo.Address))
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.FailedGenerateAddress);

                wallet.Address = addressInfo.Address;
                wallet.Tag = addressInfo.DestinationTag;

                dac.UploadAddress(wallet.Id, wallet.Address, wallet.Tag);
            }

            return new PreDepositOM
            {
                Address = wallet.Address,
                Tag = wallet.Tag,
                Code = coin.Code,
                NeedTag = coin.NeedTag
            };
        }

    }
}
