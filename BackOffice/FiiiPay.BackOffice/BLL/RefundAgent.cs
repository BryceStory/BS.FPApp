using System;
using System.Transactions;
using FiiiPay.BackOffice.Models;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.BackOffice.BLL
{
    public class RefundAgent : BaseBLL
    {
        public bool Refund(string orderNo)
        {
            var orderDac = new OrderDAC();

            var order = orderDac.GetByOrderNo(orderNo);

            if (order == null)
                throw new CommonException(10000, "Order does not exist!");

            if (order.Status != OrderStatus.Completed)
                throw new CommonException(10001, "Order state exception!");

            if (DateTime.UtcNow.AddDays(-3000) > order.PaymentTime)
                throw new CommonException(10000, "Orders cannot be refundable for more than three days!");

            var merchantWalletDAC = new MerchantWalletDAC();
            var userWalletDAC = new UserWalletDAC();

            var merchantWallet = merchantWalletDAC.GetByAccountId(order.MerchantAccountId, order.CryptoId);
            if (merchantWallet == null)
                throw new CommonException(10001, "The currency that the merchant does not support!");

            if (merchantWallet.Balance < order.ActualCryptoAmount)
                throw new CommonException(10001, "Not sufficient funds!");

            var userWallet = userWalletDAC.GetByAccountId(order.UserAccountId.Value, order.CryptoId);
            if (userWallet == null)
                throw new CommonException(10001, "A currency that is not supported by the user");

            var orderWithdrawalFee = new OrderWithdrawalFeeDAC().GetByOrderId(order.Id);
            if (orderWithdrawalFee != null)
            {
                var merchantOrderWithdrawalFeeWallet = merchantWalletDAC.GetByAccountId(order.MerchantAccountId, orderWithdrawalFee.CryptoId);
                using (var scope = new TransactionScope())
                {
                    merchantWalletDAC.Decrease(order.MerchantAccountId, order.CryptoId, order.ActualCryptoAmount);
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        WalletId = merchantWallet.Id,
                        Action = "REFUND",
                        Amount = -order.ActualCryptoAmount,
                        Balance = merchantWallet.Balance - order.ActualCryptoAmount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund to order({order.OrderNo})"
                    });

                    merchantWalletDAC.Increase(order.MerchantAccountId, orderWithdrawalFee.CryptoId, orderWithdrawalFee.Amount);
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        WalletId = merchantOrderWithdrawalFeeWallet.Id,
                        Action = "REFUND",
                        Amount = orderWithdrawalFee.Amount,
                        Balance = merchantWallet.Balance - orderWithdrawalFee.Amount,
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
                    order.Timestamp = DateTime.UtcNow;
                    orderDac.UpdateStatus(order);

                    new RefundDAC().Insert(new Refund
                    {
                        OrderId = order.Id,
                        Status = RefundStatus.Completed,
                        Timestamp = DateTime.UtcNow
                    });

                    scope.Complete();
                }
            }
            else
            {
                using (var scope = new TransactionScope())
                {
                    merchantWalletDAC.Decrease(order.MerchantAccountId, order.CryptoId, order.ActualCryptoAmount);
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
                    //order.Timestamp = DateTime.UtcNow;
                    orderDac.UpdateStatus(order);

                    new RefundDAC().Insert(new Refund
                    {
                        OrderId = order.Id,
                        Status = RefundStatus.Completed,
                        Timestamp = DateTime.UtcNow
                    });

                    scope.Complete();
                }
            }
            return true;
        }


        public bool BillerRefund(string orderNo)
        {
            var orderDac = new BillerOrderDAC();
            var order = FiiiPayDB.DB.Queryable<BillerOrders>().Where(t => t.OrderNo.Equals(orderNo)).First();
            if (order == null)
                throw new CommonException(10000, "Order does not exist!");

            var merchantWalletDAC = new MerchantWalletDAC();
            var userWalletDAC = new UserWalletDAC();

            var userWallet = userWalletDAC.GetByAccountId(order.AccountId, order.CryptoId);
            if (userWallet == null)
                throw new CommonException(10001, "A currency that is not supported by the user");
            using (var scope = new TransactionScope())
            {
                userWalletDAC.Increase(order.AccountId, order.CryptoId, order.CryptoAmount);
                new UserWalletStatementDAC().Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = "REFUND",
                    Amount = order.CryptoAmount,
                    Balance = userWallet.Balance + order.CryptoAmount,
                    Timestamp = DateTime.UtcNow,
                    Remark = $"Refund from billerorder({order.OrderNo})"
                });

                order.Status = BillerOrderStatus.Fail;
                orderDac.UpdateStatus(order);

                new RefundDAC().Insert(new Refund
                {
                    OrderId = order.Id,
                    Status = RefundStatus.Completed,
                    Timestamp = DateTime.UtcNow
                });
                scope.Complete();
            }
            return true;
        }
    }
}
