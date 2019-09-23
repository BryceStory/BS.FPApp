using System;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.DTO.Investor;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;
using TransactionStatus = FiiiPay.Framework.Enums.TransactionStatus;

namespace FiiiPay.Business
{
    public class InvestorWalletComponent
    {
        public string GetTotalAssets(InvestorAccount account)
        {
            return (account?.Balance ?? 0).ToString(2);
        }

        public string GetBanlance(InvestorAccount account)
        {
            return (account?.Balance ?? 0).ToString(FiiiCoinUtility.Cryptocurrency.DecimalPlace);
        }

        public TargetAccountDTO GetTargetAccount(int countryId, string cellphone)
        {
            CountryDAC countryDac = new CountryDAC();
            UserAccountDAC userAccountDac = new UserAccountDAC();
            Country country = countryDac.GetById(countryId);
            if (country == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.AccountNotExist);
            UserAccount userAccount = userAccountDac.GetByCountryIdAndCellphone(countryId, cellphone);
            if (userAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.AccountNotExist);

            var profile = new UserProfileAgent().GetUserProfile(userAccount.Id);
            return new TargetAccountDTO
            {
                Cellphone = GetMaskedCellphone(country.PhoneCode, userAccount.Cellphone),
                FullName = profile == null ? "" : ((string.IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName),
                Avatar = userAccount.Photo
            };
        }

        public TransferResult Transfer(InvestorAccount account, int countryId, string cellphone, decimal amount, string pinToken)
        {
            new SecurityVerification(SystemPlatform.FiiiCoinWork).VerifyToken(pinToken, SecurityMethod.Pin);

            CountryDAC countryDac = new CountryDAC();
            UserAccountDAC userAccountDac = new UserAccountDAC();
            Country country = countryDac.GetById(countryId);
            if (country == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.AccountNotExist);
            UserAccount userAccount = userAccountDac.GetByCountryIdAndCellphone(countryId, cellphone);
            if (userAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.AccountNotExist);

            InvestorAccountDAC accountDac = new InvestorAccountDAC();
            if (account.Balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.余额不足);

            UserWalletDAC userWalletDac = new UserWalletDAC();

            UserWallet userWallet = userWalletDac.GetByAccountId(userAccount.Id, FiiiCoinUtility.Cryptocurrency.Id);

            InvestorOrder investorOrder;
            UserDeposit userDeposit;
            using (var scope = new TransactionScope())
            {
                if (userWallet == null)
                    userWallet = new UserWalletComponent().GenerateWallet(userAccount.Id, FiiiCoinUtility.Cryptocurrency.Id);
                accountDac.Decrease(account.Id, amount);
                investorOrder = new InvestorOrderDAC().Insert(new InvestorOrder
                {
                    Id = Guid.NewGuid(),
                    OrderNo = CreateOrderNo(),
                    TransactionType = InvestorTransactionType.Transfer,
                    Status = 1,
                    InverstorAccountId = account.Id,
                    UserAccountId = userAccount.Id,
                    CryptoId = FiiiCoinUtility.Cryptocurrency.Id,
                    CryptoAmount = amount,
                    Timestamp = DateTime.UtcNow
                });
                new InvestorWalletStatementDAC().Insert(new InvestorWalletStatement
                {
                    Id = Guid.NewGuid(),
                    InvestorId = account.Id,
                    TagAccountId = userAccount.Id,
                    Action = InvestorTransactionType.Transfer,
                    Amount = -amount,
                    Balance = account.Balance - amount,
                    Timestamp = DateTime.UtcNow
                });
                // 2018-06-26: new rules IncreaseFrozen -> Increase
                userWalletDac.Increase(userWallet.Id, amount);
                userDeposit = new UserDepositDAC().Insert(new UserDeposit
                {
                    UserAccountId = userAccount.Id,
                    UserWalletId = userWallet.Id,
                    FromAddress = null,
                    FromTag = null,
                    ToAddress = null,
                    ToTag = null,
                    Amount = amount,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    TransactionId = account.Id.ToString(),
                    SelfPlatform = true,
                    RequestId = null
                });


                scope.Complete();
            }

            InvestorMSMQ.PubUserDeposit(userDeposit.Id,0);

            return new TransferResult
            {
                OrderId = investorOrder.Id,
                OrderNo = investorOrder.OrderNo,
                TargetAccount = GetMaskedCellphone(country.PhoneCode, userAccount.Cellphone),
                Timestamp = DateTime.UtcNow.ToUnixTime()
            };
        }


        private string CreateOrderNo()
        {
            return DateTime.Now.ToUnixTime() + new Random().Next(0, 100).ToString("00");
        }

        private string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }
    }
}