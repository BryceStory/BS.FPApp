using FiiiPay.Data;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Transactions;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Framework.Component;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Queue;
using FiiiPay.Framework.Exceptions;
using System.Linq;
using FiiiPay.Foundation.Business;

namespace FiiiPOS.Web.Business
{
    public class OrderComponent
    {
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="orderSN"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<OrderByPage> GetOrderRecordList(Guid merchantId, string orderSN, int status, string startDate, string endDate, int pageIndx, int pageSize, out int count)
        {
            count = 0;
            List<OrderByPage> list = new OrderDAC().GetOrderRecordListByPage(merchantId, orderSN, status, startDate, endDate, pageIndx, pageSize);
            foreach (var item in list)
            {
                var coin = new CryptocurrencyDAC().GetById(item.CryptoId);
                var er = item.ExchangeRate;
                var cer = GetExchangeRate(item.FiatCurrency, coin);
                var iRate = ((cer - er) / er) * 100;

                item.CurrentExchangeRate = $"1 {coin.Code} = {cer.ToString(2)} {item.FiatCurrency}";
                item.IncreaseRate = iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2);
            }

            if (list != null && list.Count > 0)
                count = list[0].TotalCount;

            return list;
        }

        /// <summary>
        /// 订单退款
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="orderNo"></param>
        /// <param name="pin"></param>
        /// <returns>-1=账号不存在 -2=PIN码错误 -3=订单不存在 -4=商家账户不对 -5=订单状态不符合要求 -6=商家不支持的币种 -7=商家金额不够 -8=用户钱包不支持币种 -9=回滚 -10=日期超过3天</returns>
        public int RefundOrder(Guid merchantId, string orderNo, string pin)
        {
            OrderDAC orderDac = new OrderDAC();

            MerchantAccount merchantAccount = new MerchantAccountDAC().GetById(merchantId);
            if (merchantAccount == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ACCOUNT_NOT_EXISTS, "账号不存在");

            if (!PasswordHasher.VerifyHashedPassword(merchantAccount.PIN, pin))
                throw new CommonException(ReasonCode.PIN_ERROR, "PIN码错误");   //Wrong PIN enterred

            Order order = orderDac.GetByOrderNo(orderNo);
            if (order == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ORDERNO_NOT_EXISTS, "找不到该订单"); //The order not exist

            if (merchantAccount.Id != order.MerchantAccountId)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ORDERNO_NOTBE_ACCOUNT, "不是该用户的订单");

            if (order.Status != OrderStatus.Completed)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.ORDER_COMPLETED, "订单已完成,不能退款");  //Wrong order status.

            if (DateTime.UtcNow.AddDays(-3) > order.PaymentTime.Value)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_CODE_EXPIRE, "订单支付已超过3天,不能退款");//订单超过3天

            var merchantWalletDAC = new MerchantWalletDAC();
            var userWalletDAC = new UserWalletDAC();

            var merchantWallet = merchantWalletDAC.GetByAccountId(order.MerchantAccountId, order.CryptoId);
            if (merchantWallet == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.REFUND_MERCHANT_WALLET_NOT_EXISTS, "找不到该商家的钱包"); //Merchant account not support this cryptocurrency

            if (merchantWallet.Balance < order.ActualCryptoAmount)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.REFUND_BALANCE_LOW, "余额不足,无法退款");  //Balance not enough

            var userWallet = userWalletDAC.GetByAccountId(order.UserAccountId.Value, order.CryptoId);
            if (userWallet == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.REFUND_USER_WALLET_NOT_EXISTS, "找不到订单用户的钱包");  //User account not support this cryptocurrency

            int result = -9;
            using (var scope = new TransactionScope())
            {
                merchantWalletDAC.Decrease(merchantAccount.Id, order.CryptoId, order.ActualCryptoAmount);
                new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                {
                    WalletId = merchantWallet.Id,
                    Action = "REFUND",
                    Amount = -order.ActualCryptoAmount,
                    Balance = merchantWallet.Balance - order.ActualCryptoAmount,
                    Timestamp = DateTime.UtcNow,
                    Remark = $"Refund to order({order.OrderNo})"
                });

                userWalletDAC.Increase(order.UserAccountId.Value, order.CryptoId, order.CryptoAmount);
                new UserWalletStatementDAC().Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = "REFUND",
                    Amount = order.CryptoAmount,
                    Balance = userWallet.Balance + order.CryptoAmount,
                    Timestamp = DateTime.UtcNow,
                    Remark = $"Refund from order({order.OrderNo})"
                });

                order.Status = OrderStatus.Refunded;

                orderDac.UpdateStatus(order);

                new RefundDAC().Insert(new Refund
                {
                    OrderId = order.Id,
                    Status = RefundStatus.Completed,
                    Timestamp = DateTime.UtcNow
                });

                result = 1;
                scope.Complete();
            }

            if (result > 0) //发送退款通知
            {
                RabbitMQSender.SendMessage("RefundOrder", order.OrderNo);
            }

            return result;
        }


        /// <summary>
        /// 获取区间的日期订单统计[周、月]
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<OrderDayStat> GetOrderStatInDay(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            OrderDAC dac = new OrderDAC();
            List<OrderDayStat> list = dac.GetTradingStatInDay(merchantId, startDate, endDate);
            return list;
        }

        /// <summary>
        /// 获取区间的日期订单统计[年]
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<OrderMonthStat> GetOrderStatInMonth(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            OrderDAC dac = new OrderDAC();
            List<OrderMonthStat> list = dac.GetTradingStatInMonth(merchantId, startDate, endDate);
            return list;
        }

        /// <summary>
        /// 统计今天、所有交易的订单量和金额
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public OrderStat GetOrderStat(Guid merchantId, int status = 2)
        {
            OrderDAC dac = new OrderDAC();
            OrderStat orderStat = dac.GetOrderStat(merchantId, status);
            return orderStat;
        }
        private decimal GetExchangeRate(string fiatCurrency, Cryptocurrency crypto)
        {
            var price = new MarketPriceComponent().GetMarketPrice(fiatCurrency, crypto.Code);
            return price?.Price ?? 0;
        }

        public string DayActualReceipt(Guid accountId, DateTime date)
        {
            return new OrderDAC().DayOfOrderAmount(accountId, date).ToString(2) ?? "0.00";
        }
        public string AllActualReceipt(Guid accountId)
        {
            return new OrderDAC().ALLOfOrderAmount(accountId).ToString(2) ?? "0.00";
        }
        public string MonthActualReceipt(Guid accountId, DateTime month)
        {
            return new OrderDAC().MonthOfOrderAmount(accountId, month).ToString(2) ?? "0.00";
        }
        public string ActualReceipt(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return new OrderDAC().DateOfOrderAmount(accountId, startDate, endDate).ToString(2) ?? "0.00";
        }
    }
}
