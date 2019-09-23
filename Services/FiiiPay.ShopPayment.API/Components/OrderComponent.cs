using System;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;
using FiiiPay.ShopPayment.API.Models;
using FiiiPay.ShopPayment.API.Properties;
using log4net;

namespace FiiiPay.ShopPayment.API.Components
{
    internal class OrderComponent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(OrderComponent));

        private const int AccountNotFound = 10001;
        private const int WalletNotFound = 10002;
        private const int InsufficientBalance = 10003;
        private const int PaymentError = 10004;
        private const int OrderExist = 10005;
        private const int PINError = 10006;

        private const int TradeAmountError = 10011;
        private const int RefundedError = 10012;
        private const int OrderNoTrade = 10013;

        public ResultDto Payment(PaymentVo model)
        {
            var result = new ResultDto();

            var userAccountId = model.UserId;

            var userAccountDac = new UserAccountDAC();
            var userWalletDac = new UserWalletDAC();
            var mallDac = new MallPaymentOrderDAC();

            var account = userAccountDac.GetById(userAccountId);
            if (account == null)
            {
                result.Code = AccountNotFound;
                result.Message = Resource.AccountNotFound;

                return result;
            }

            if (!PasswordHasher.VerifyHashedPassword(account.Pin, model.PIN))
            {
                result.Code = PINError;
                result.Message = Resource.PINError;

                return result;
            }

            var wallet = userWalletDac.GetByCryptoCode(userAccountId, "FIII");
            if (wallet == null)
            {
                result.Code = WalletNotFound;
                result.Message = Resource.WalletNotFound;

                return result;
            }

            if (wallet.Balance < model.CryptoAmount)
            {
                result.Code = InsufficientBalance;
                result.Message = Resource.InsufficientBalance;

                return result;
            }

            var extisorder = mallDac.GetByOrderId(model.OrderId);
            if (extisorder != null && extisorder.Status == OrderStatus.Completed)
            {
                result.Code = OrderExist;
                result.Message = Resource.OrderExist;

                return result;
            }

            try
            {
                if (extisorder == null)
                {
                    var order = new MallPaymentOrder
                    {
                        Id = Guid.NewGuid(),
                        CryptoAmount = model.CryptoAmount,
                        ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                        OrderId = model.OrderId,
                        Remark = "",
                        Status = OrderStatus.Pending,
                        Timestamp = DateTime.UtcNow,
                        UserAccountId = userAccountId,
                        RefundTradeNo = string.Empty,
                        TradeNo = string.Empty
                    };
                    mallDac.Create(order);
                    RabbitMQSender.SendMessage("PaymentGatewayPayOrder", order);
                }
                else
                {
                    RabbitMQSender.SendMessage("PaymentGatewayPayOrder", extisorder);
                }

                result.Message = Resource.Success;
                return result;
            }
            catch (Exception exception)
            {
                _log.Error(exception);

                result.Code = PaymentError;
                result.Message = Resource.PaymentError;

                return result;
            }
        }

        public ResultDto Refund(RefundVo model)
        {
            var result = new ResultDto();

            var mallDac = new MallPaymentOrderDAC();
            var order = mallDac.GetByOrderId(model.OrderId);

            if (order.Status != OrderStatus.Completed)
            {
                result.Code = OrderNoTrade;
                result.Message = Resource.OrderNoTrade;
                return result;
            }

            if (order.Status == OrderStatus.Refunded)
            {
                result.Code = RefundedError;
                result.Message = Resource.RefundedError;
                return result;
            }

            if (order.CryptoAmount != model.CryptoAmount)
            {
                result.Code = TradeAmountError;
                result.Message = Resource.TradeAmountError;
                return result;
            }

            RabbitMQSender.SendMessage("PaymentGatewayRefund", order);

            result.Message = Resource.Success;
            return result;
        }
    }
}