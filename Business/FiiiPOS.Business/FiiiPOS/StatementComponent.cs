using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiiiPay.Data;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;
using log4net;
using Newtonsoft.Json;

namespace FiiiPOS.Business.FiiiPOS
{
    public class StatementComponent
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(StatementComponent));
        public List<OrderDTO> ReceiptStatement(Guid accountId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var orderDac = new OrderDAC();
            _logger.Info("----------------" + accountId + startTime + pageIndex + pageSize);
            var orderList = orderDac.GetMerchantOrderByDate(accountId, startTime,endTime, pageIndex, pageSize);
            _logger.Info("----------------" + JsonConvert.SerializeObject(orderList));
            var coinList = new CryptocurrencyDAC().GetAll();
            var result = new List<OrderDTO>();

            foreach (var order in orderList)
            {
                var coin = coinList.FirstOrDefault(e => e.Id == order.CryptoId);
                var orderDTO = new OrderDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderStatus = order.Status,
                    Timestamp = order.PaymentTime.Value.ToUtcTimeTicks(),
                    CryptoCode = coin?.Code,
                    CryptoAmount = order.CryptoAmount.ToString(coin?.DecimalPlace ?? 0),
                    FiatCurrency = order.FiatCurrency,
                    FiatAmount = order.FiatAmount.ToString("F", CultureInfo.InvariantCulture),
                    UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                    ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin?.DecimalPlace ?? 0)
                };
                result.Add(orderDTO);
            }

            return result;
        }


        public List<OrderDTO> SearchReceiptStatement(Guid accountId, string orderno, int pageIndex, int pageSize)
        {
            var orderDac = new OrderDAC();

            var orderList = orderDac.GetMerchantOrderByOrderNo(accountId, orderno, pageIndex, pageSize);

            var coinList = new CryptocurrencyDAC().GetAll();

            var result = new List<OrderDTO>();

            foreach (var order in orderList)
            {
                var coin = coinList.FirstOrDefault(e => e.Id == order.CryptoId);
                var orderDTO = new OrderDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderStatus = order.Status,
                    Timestamp = order.Timestamp.ToUnixTime(),
                    CryptoCode = coin?.Code,
                    CryptoAmount = order.CryptoAmount.ToString(coin?.DecimalPlace ?? 0),
                    FiatCurrency = order.FiatCurrency,
                    FiatAmount = order.FiatAmount.ToString("F", CultureInfo.InvariantCulture),
                    UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                    ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin?.DecimalPlace ?? 0)
                };
                result.Add(orderDTO);
            }

            return result;
        }

        //public int TotalReceiptCount(Guid accountId, DateTime date)
        //{
        //    return new OrderDAC().DayOfOrderList(accountId, date)?.Count ?? 0;
        //}

        public string ActualReceipt(Guid accountId, DateTime startTime, DateTime endTime)
        {
            var amount = new OrderDAC().DayOfActualReceipt(accountId, startTime, endTime);
            return amount.ToString(2);
        }


        public List<OrderDTO> RefundStatement(Guid accountId, int pageIndex, int pageSize)
        {
            var orderDac = new OrderDAC();
            var orderList = orderDac.GetMerchantRefundOrder(accountId, pageIndex, pageSize);
            var coinList = new CryptocurrencyDAC().GetAll();

            var result = new List<OrderDTO>();

            foreach (var order in orderList)
            {
                var coin = coinList.FirstOrDefault(e => e.Id == order.CryptoId);
                var orderDTO = new OrderDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    OrderStatus = order.Status,
                    Timestamp = order.Timestamp.ToUnixTime(),
                    CryptoCode = coin?.Code,
                    CryptoAmount = order.CryptoAmount.ToString(coin?.DecimalPlace ?? 0),
                    FiatCurrency = order.FiatCurrency,
                    FiatAmount = order.FiatAmount.ToString("F", CultureInfo.InvariantCulture),
                    UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                    ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin?.DecimalPlace ?? 0)
                };
                result.Add(orderDTO);
            }

            return result;
        }

        public List<MerchantTransferDTO> TransferStatement(Guid merchantAccountId, long merchantWalletId, int pageIndex, int pageSize)
        {
            var dac = new MerchantWalletDAC();
            var list = dac.GetMerchantTransferStatementById(merchantAccountId, merchantWalletId, pageIndex, pageSize);

            var cryptoList = new CryptocurrencyDAC().GetAll();

            return list.Select(e =>
            {
                var crypto = cryptoList.FirstOrDefault(c => c.Id == e.CryptoId);

                return new MerchantTransferDTO
                {
                    Id = e.Id,
                    TransactionType = e.TransferType,
                    TransactionStatus = e.Status,
                    CryptoCode = crypto?.Code,
                    Amount = e.Amount.ToString(crypto?.DecimalPlace ?? 0),
                    Timestamp = e.Timestamp.ToUnixTime()
                };
            }).ToList();
        }

        public List<MerchantWithdrawalDTO> WithdrawalStatement(Guid accountId, int pageIndex, int pageSize)
        {
            var dac = new MerchantWithdrawalDAC();
            var list = dac.GetByMerchantAccountId(accountId, pageIndex, pageSize);
            var cryptoList = new CryptocurrencyDAC().GetAll();
            return list.Select(e =>
            {
                var crypto = cryptoList.FirstOrDefault(c => c.Id == e.CryptoId);
                return new MerchantWithdrawalDTO
                {
                    Id = e.Id,
                    TransactionType = TransactionType.Withdrawal,
                    TransactionStatus = e.Status == TransactionStatus.UnSubmit ? TransactionStatus.Pending : e.Status,
                    Address = e.Address,
                    Amount = (e.Amount - e.WithdrawalFee).ToString(crypto?.DecimalPlace ?? 0),
                    Timestamp = e.Timestamp.ToUnixTime(),
                    OrderNo = e.OrderNo,
                    TransactionFee = e.WithdrawalFee.ToString(CultureInfo.InvariantCulture),
                    CryptoCode = crypto?.Code
                };
            }).ToList();
        }

        public List<MerchantDepositDTO> DepositStatement(Guid accountId, int pageIndex, int pageSize)
        {
            var dac = new MerchantDepositDAC();
            var list = dac.GetByMerchantAccountId(accountId, pageIndex, pageSize);
            var cryptoList = new CryptocurrencyDAC().GetAll();
            return list.Select(e =>
            {
                var crypto = cryptoList.FirstOrDefault(c => c.Id == e.CryptoId);

                return new MerchantDepositDTO
                {
                    Id = e.Id,
                    TransactionType = TransactionType.Deposit,
                    TransactionStatus = e.Status,
                    //Address = e.FromAddress,
                    Amount = e.Amount.ToString(crypto?.DecimalPlace ?? 8),
                    Timestamp = e.Timestamp.ToUnixTime(),
                    OrderNo = e.OrderNo,
                    CryptoCode = crypto?.Code
                };
            }).ToList();
        }


        public MerchantWithdrawalDTO WithdrawalDetail(Guid accountId, long withdrawalId)
        {
            var dac = new MerchantWithdrawalDAC();
            var account = new MerchantAccountDAC().GetById(accountId);
            var data = dac.GetById(accountId, withdrawalId);
            if (data == null)
                return null;

            var crypto = new CryptocurrencyDAC().GetById(data.CryptoId);
            var result = new MerchantWithdrawalDTO
            {
                TransactionType = TransactionType.Withdrawal,
                TransactionStatus = data.Status == TransactionStatus.UnSubmit ? TransactionStatus.Pending : data.Status,
                Address = data.Address,
                Tag = data.Tag,
                NeedTag = crypto.NeedTag,
                Amount = (data.Amount - data.WithdrawalFee).ToString(crypto.DecimalPlace),
                Timestamp = data.Timestamp.ToUnixTime(),
                OrderNo = data.OrderNo,
                TransactionFee = data.WithdrawalFee.ToString(crypto.DecimalPlace),
                CryptoCode = crypto.Code,
                SelfPlatform = data.SelfPlatform,
                TransactionId = data.SelfPlatform ? "-" : data.TransactionId ?? "-",
                Remark = data.Remark,
                AvatarId = account.Photo
            };

            bool showCheckTime = crypto.Code != "XRP";

            if (showCheckTime && !data.SelfPlatform &&
                result.TransactionStatus == TransactionStatus.Pending &&
                data.RequestId.HasValue)
            {
                var statusInfo = new FiiiFinanceAgent().GetStatus(data.RequestId.Value);
                result.CheckTime = $"{statusInfo.TotalConfirmation}/{statusInfo.MinRequiredConfirmation}";
                result.TransactionId = statusInfo.TransactionID;
            }

            return result;
        }

        public MerchantDepositDTO DepositDetail(Guid accountId, long depositId)
        {
            var dac = new MerchantDepositDAC();
            var account = new MerchantAccountDAC().GetById(accountId);
            var data = dac.GetById(accountId, depositId);
            if (data == null)
                return null;
            var crypto = new CryptocurrencyDAC().GetById(data.CryptoId);
            var result = new MerchantDepositDTO
            {
                Id = data.Id,
                TransactionType = TransactionType.Deposit,
                TransactionStatus = data.Status,
                //Address = data.FromAddress,
                Amount = data.Amount.ToString(crypto.DecimalPlace),
                //FaitAmount = (markPrice.Price * data.Amount.Value).ToString("F", CultureInfo.InvariantCulture),
                //FaitCurrency = account.FiatCurrency,
                Timestamp = data.Timestamp.ToUnixTime(),
                OrderNo = data.OrderNo,
                CryptoCode = crypto.Code,
                SelfPlatform = data.SelfPlatform,
                TransactionId = data.SelfPlatform ? "-" : data.TransactionId ?? "-"
            };

            if (!data.SelfPlatform &&
                result.TransactionStatus == TransactionStatus.Pending &&
                data.RequestId.HasValue)
            {
                var statusInfo = new FiiiFinanceAgent().GetDepositStatus(data.RequestId.Value);
                result.CheckTime = $"{statusInfo.TotalConfirmation}/{statusInfo.MinRequiredConfirmation}";
            }

            return result;
        }


        private string GetUserMastMaskedCellphone(Guid userAccountId)
        {
            var user = new UserAccountDAC().GetById(userAccountId);
            var country = new CountryComponent().GetById(user.CountryId);

            return CellphoneExtension.GetMaskedCellphone(country.PhoneCode, user.Cellphone);
        }

    }
}