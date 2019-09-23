using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MallPaymentOrderDAC : BaseDataAccess
    {
        public void Create(MallPaymentOrder model)
        {
            const string sql =
                "INSERT INTO MallPaymentOrder(Id, OrderId, TradeNo, UserAccountId, CryptoAmount, Status, ExpiredTime,Timestamp, Remark) VALUES(@Id, @OrderId, @TradeNo, @UserAccountId, @CryptoAmount, @Status,@ExpiredTime, @Timestamp, @Remark);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, model);
            }
        }

        public void UpdateStatus(Guid id, byte status)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [Status] = @Status
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id,
                    Status = status
                });
            }
        }

        public void UpdateTradeNo(Guid id, string tradeNo)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [TradeNo] = @TradeNo
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id,
                    TradeNo = tradeNo
                });
            }
        }

        public void UpdateNotification(Guid id)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [HasNotification] = 1
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id
                });
            }
        }

        public void UpdateRefundNotification(Guid id)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [RefundHasNotification] = 1
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id
                });
            }
        }

        public void UpdateNotificationSource(Guid id, string notificationSource)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [NotificationSource] = @NotificationSource
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id,
                    NotificationSource = notificationSource
                });
            }
        }

        public void UpdateRefundTradeNo(Guid id, string refundTradeNo)
        {
            const string sql = @"UPDATE [dbo].[MallPaymentOrder]
                                    SET [RefundTradeNo] = @RefundTradeNo
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id,
                    RefundTradeNo = refundTradeNo
                });
            }
        }

        public MallPaymentOrder GetByOrderId(string id)
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE OrderId=@OrderId";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MallPaymentOrder>(sql, new { OrderId = id });
            }
        }

        public MallPaymentOrder GetByTradeNo(string id)
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE TradeNo=@TradeNo";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MallPaymentOrder>(sql, new { TradeNo = id });
            }
        }

        public MallPaymentOrder GetById(Guid id)
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MallPaymentOrder>(sql, new { Id = id });
            }
        }

        public List<MallPaymentOrder> GetNotification(bool hasNotification = false)
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE [HasNotification]=@HasNotification";

            using (var con = ReadConnection())
            {
                return con.Query<MallPaymentOrder>(sql, new { HasNotification = hasNotification }).ToList();
            }
        }

        public bool HasNotifiation(Guid id)
        {
            const string sql = @"SELECT HasNotification FROM MallPaymentOrder WHERE [Id]=@Id";

            using (var con = ReadConnection())
            {
                var data = con.QueryFirstOrDefault<MallPaymentOrder>(sql, new { Id = id });
                if (data == null) return false;

                return data.HasNotification;
            }
        }

        public bool RefundHasNotifiation(Guid id)
        {
            const string sql = @"SELECT RefundHasNotification FROM MallPaymentOrder WHERE [Id]=@Id";

            using (var con = ReadConnection())
            {
                var data = con.QueryFirstOrDefault<MallPaymentOrder>(sql, new { Id = id });
                if (data == null) return false;

                return data.RefundHasNotification;
            }
        }

        public List<MallPaymentOrder> GetNotificationError()
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE [HasNotification] = 0 and (NotificationSource = '' or NotificationSource is not null) ";

            using (var con = ReadConnection())
            {
                return con.Query<MallPaymentOrder>(sql).ToList();
            }
        }

        public List<MallPaymentOrder> GetRefundNotifitaionError()
        {
            const string sql = @"SELECT * FROM MallPaymentOrder WHERE [HasNotification] = 1 and RefundHasNotification = 0 and (NotificationSource = '' or NotificationSource is not null)";

            using (var con = ReadConnection())
            {
                return con.Query<MallPaymentOrder>(sql).ToList();
            }
        }
    }
}
