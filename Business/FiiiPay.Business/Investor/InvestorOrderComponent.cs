using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.DTO.Investor;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;

namespace FiiiPay.Business
{
    public class InvestorOrderComponent
    {
        public List<StatementDTO> List(long accountId, long? startTimestamp, long? endTimestamp,
            InvestorTransactionType? transactionType, string cellphone, int pageIndex, int pageSize)
        {
            InvestorOrderDAC orderDac = new InvestorOrderDAC();
            var results = string.IsNullOrEmpty(cellphone)
                ? orderDac.SelectAndStatments(accountId, startTimestamp, endTimestamp, pageIndex, pageSize, transactionType)
                : orderDac.SelectByUserCellphone(accountId, startTimestamp, endTimestamp, cellphone, pageIndex, pageSize);

            return results.Select(e => new StatementDTO
            {
                Id = e.Id,
                TransactionType =  e.TransactionType,
                Amount = e.Amount.ToString(FiiiCoinUtility.Cryptocurrency.DecimalPlace),
                Timestamp = e.Timestamp.ToUnixTime()
            }).ToList();
        }

        public OrderDetailDTO OrderDetail(long accountId, Guid orderId)
        {
            InvestorOrder order = new InvestorOrderDAC().GetById(accountId, orderId);
            if (order == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.订单不存在);

            UserAccount userAccount = new UserAccountDAC().GetById(order.UserAccountId);
            if (userAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.用户不存在);
            Country country = new CountryDAC().GetById(userAccount.CountryId);
            if (country == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.用户不存在);


            var profile = new UserProfileAgent().GetUserProfile(userAccount.Id);

            Cryptocurrency crypto = new CryptocurrencyDAC().GetById(order.CryptoId);
            return new OrderDetailDTO
            {
                OrderId = order.Id,
                OrderNo = order.OrderNo,
                TransactionType = order.TransactionType,
                Status = order.Status,
                CryptoCode = crypto.Code,
                Amount = order.CryptoAmount.ToString(crypto.DecimalPlace),
                UserAccount = GetMaskedCellphone(country.PhoneCode, userAccount.Cellphone),
                Username = profile == null ? "" : ((string.IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName),
                Timestamp = order.Timestamp.ToUnixTime()
            };
        }

        public StatementDetailDTO StatementDetail(long accountId, Guid statementId)
        {
            InvestorWalletStatementDAC dac = new InvestorWalletStatementDAC();
            var data = dac.GetById(accountId, statementId);
            if (data == null)
                return null;
            return new StatementDetailDTO
            {
                Id = data.Id,
                TransactionType = data.Action,
                Amount = data.Amount.ToString(FiiiCoinUtility.Cryptocurrency.DecimalPlace),
                Timestamp = data.Timestamp.ToUnixTime(),
                Remark = data.Remark
            };
        }

        private string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }
    }
}