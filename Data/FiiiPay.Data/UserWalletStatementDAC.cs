using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using FiiiPay.Entities.EntitySet;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class UserWalletStatementDAC : BaseDataAccess
    {
        public long Insert(UserWalletStatement userWalletStatement)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>("INSERT INTO UserWalletStatements(WalletId,Action,Amount,Balance,FrozenAmount,FrozenBalance,Timestamp,Remark) VALUES(@WalletId,@Action,@Amount,@Balance,@FrozenAmount,@FrozenBalance,@Timestamp,@Remark); SELECT SCOPE_IDENTITY()", userWalletStatement);
            }
        }

        public async Task<List<UserWalletStatementES>> ListAllTypeAsync(Guid UserAccountId, int? coinId, int pageSize, int pageIndex, string mounth, string startDate, string endDate, string keyword, int maxType)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT a.* FROM (
                            	SELECT 0 [Type],CONVERT(VARCHAR(50),ud.Id) [OrderId],ud.[Status],ud.[Timestamp],ud.[Amount] [CryptoAmount],uw.[CryptoId],ud.[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [UserDeposits] ud 
                                  JOIN [UserWallets] uw ON ud.UserWalletId=uw.Id 
                                 WHERE ud.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 1 [Type],CONVERT(VARCHAR(50),uwd.Id) [OrderId],uwd.[Status],uwd.[Timestamp],(uwd.[Amount]-uwf.[Fee]) [CryptoAmount],uw.[CryptoId],uwd.[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [UserWithdrawals] uwd 
                                  JOIN UserWallets uw ON uwd.UserWalletId=uw.Id 
                                  JOIN UserWithdrawalFee uwf ON uwf.WithdrawalId=uwd.Id
                                 WHERE uwd.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 2 [Type],CONVERT(VARCHAR(50),o.Id) OrderId,o.[Status],o.[Timestamp],o.[CryptoAmount],o.[CryptoId],o.[OrderNo],ma.[MerchantName],0 [RefundStatus] FROM [Orders] o 
                                  JOIN MerchantAccounts ma ON o.MerchantAccountId=ma.Id
                                 WHERE o.[Status]=2 AND o.UserAccountId=@UserAccountId
                             UNION ALL
                                 SELECT 12 AS [Type],CONVERT(VARCHAR(50),[Id]) AS OrderId,[Status],[Timestamp],[CryptoAmount],[CryptoId],[OrderNo],[MerchantInfoName] AS MerchantName,0 [RefundStatus] FROM [dbo].[StoreOrders] WHERE [UserAccountId]=@UserAccountId
                             UNION ALL
                                SELECT 3 [Type],CONVERT(VARCHAR(50),o.Id) OrderId,o.[Status],r.[Timestamp],o.[CryptoAmount],o.[CryptoId],o.[OrderNo],ma.[MerchantName],0 [RefundStatus] FROM Refunds r 
                                  JOIN Orders o ON r.OrderId = o.Id 
                                  JOIN MerchantAccounts ma ON o.MerchantAccountId=ma.Id
                                 WHERE o.[Status]=3 AND o.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 4 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp], [Amount] [CryptoAmount],[CoinId] [CryptoId],[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [dbo].[UserTransfers]  
                                 WHERE FromUserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 5 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp], [Amount] [CryptoAmount],[CoinId] [CryptoId],[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [dbo].[UserTransfers]  
                                 WHERE ToUserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 6 [Type],CONVERT(VARCHAR(50),uto.Id) OrderId,uto.[Status],uto.[Timestamp],uto.Amount CryptoAmount,uto.CryptoId,[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM UserExTransferOrders uto
                                 WHERE OrderType=1 AND AccountId=@UserAccountId
                             UNION ALL
                                SELECT 7 [Type],CONVERT(VARCHAR(50),uto.Id) OrderId,uto.[Status],uto.[Timestamp],uto.Amount CryptoAmount,uto.CryptoId,[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM UserExTransferOrders uto
                                 WHERE OrderType=2 AND AccountId=@UserAccountId
                             UNION ALL
                                SELECT 8 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp],[CryptoAmount],[CryptoId],[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [ProfitDetails] 
                                 WHERE AccountId=@UserAccountId
                             UNION ALL
                                SELECT 9 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwo.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName],0 [RefundStatus] FROM [GatewayOrders] gwo 
                                 WHERE gwo.[Status]=2 AND gwo.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 10 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwr.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName],0 [RefundStatus] FROM [GatewayRefundOrders] gwr 
                                  JOIN GatewayOrders gwo ON gwr.OrderId = gwo.Id 
                                 WHERE gwo.[Status]=3 AND gwo.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 11 [Type],CONVERT(VARCHAR(50),bo.Id) OrderId,bo.[Status],bo.[Timestamp],bo.[CryptoAmount],bo.[CryptoId],bo.[OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [BillerOrders] bo
                                 WHERE bo.AccountId=@UserAccountId
                             UNION ALL
                                SELECT 13 AS [Type],CONVERT(VARCHAR(50),a.[Id]) AS OrderId,a.[Status],a.[Timestamp],a.[CryptoActualAmount] AS CryptoAmount,a.[CryptoId],a.[OrderNo],a.[MerchantInfoName] AS MerchantName,0 [RefundStatus] FROM [dbo].[StoreOrders] a
                                    LEFT JOIN [dbo].[MerchantInformations] b ON a.[MerchantInfoId]=b.Id WHERE b.[MerchantAccountId]=@UserAccountId
                             UNION ALL
                                SELECT 14 AS [Type],CONVERT(VARCHAR(50),[Id]) AS OrderId,[Status],[Timestamp],[Amount] AS CryptoAmount,[CryptoId],NULL [OrderNo],NULL [MerchantName],
                                    (CASE WHEN [Status]> 1 THEN(CASE WHEN[Balance] = 0 THEN 0 WHEN[Amount] >[Balance] THEN 1 WHEN[Amount] =[Balance] THEN 2 ELSE 0 END) ELSE 0 END) RefundStatus
                                    FROM[dbo].[RedPockets] WHERE [AccountId]=@UserAccountId
                             UNION ALL
                                SELECT 15 AS [Type],CONVERT(VARCHAR(50),a.[Id]) AS OrderId,a.[Status],b.[Timestamp],b.[Amount] AS CryptoAmount,a.[CryptoId],NULL [OrderNo],NULL [MerchantName],0 [RefundStatus] FROM [dbo].[RedPockets] a
                                     LEFT JOIN [dbo].[RedPocketReceivers] b ON a.Id=b.PocketId
                                     WHERE b.[AccountId]=@UserAccountId
                            ) a  WHERE 1=1 ");
            if (!string.IsNullOrEmpty(keyword))
            {
                sb.AppendLine("AND ([MerchantName] LIKE @keyword OR [OrderNo] LIKE @keyword)");
            }
            if (coinId.HasValue)
            {
                sb.AppendLine($"AND [CryptoId] = {coinId}");
            }
            if (!string.IsNullOrEmpty(mounth))
            {
                var arr = mounth.Split('-');
                var yy = int.Parse(arr[0]);
                var mm = int.Parse(arr[1]);
                sb.AppendLine($"AND DATEPART(YY,[Timestamp]) = {yy}");
                sb.AppendLine($"AND DATEPART(MM,[Timestamp]) = {mm}");
            }
            else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                if (endDate.Length == 10)
                {
                    endDate += " 23:59:59";
                }
                sb.AppendLine("AND [Timestamp] > @startDate");
                sb.AppendLine("AND [Timestamp] <= @endDate");
            }
            sb.AppendLine($"AND [Type] <= {maxType}");

            sb.AppendLine("ORDER BY [Timestamp] DESC");
            sb.AppendLine($"OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            var sql = sb.ToString();

            using (var con = await ReadConnectionAsync())
            {
                var list = await con.QueryAsync<UserWalletStatementES>(sql, new { UserAccountId, startDate, endDate, keyword = string.IsNullOrEmpty(keyword) ? null : $"%{keyword}%" });
                return list.AsList();
            }
        }

        //12 门店消费 13门店收入 14发红包 15领红包
        public async Task<List<UserWalletStatementES>> ListSingleTypeAsync(Guid UserAccountId,int type, int maxType, int? coinId, int pageSize, int pageIndex, string mounth, string startDate, string endDate, string keyword)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT a.* FROM (");
            switch (type)
            {
                case 0:
                    sb.AppendLine(@"SELECT 0 [Type],CONVERT(VARCHAR(50),ud.Id) [OrderId],ud.[Status],ud.[Timestamp],ud.[Amount] [CryptoAmount],uw.[CryptoId],ud.[OrderNo],NULL [MerchantName] FROM [UserDeposits] ud 
                                  JOIN[UserWallets] uw ON ud.UserWalletId = uw.Id
                                 WHERE ud.UserAccountId = @UserAccountId");
                    break;
                case 1:
                    sb.AppendLine(@"SELECT 1 [Type],CONVERT(VARCHAR(50),uwd.Id) [OrderId],uwd.[Status],uwd.[Timestamp],(uwd.[Amount]-uwf.[Fee]) [CryptoAmount],uw.[CryptoId],uwd.[OrderNo],NULL [MerchantName] FROM [UserWithdrawals] uwd 
                                  JOIN UserWallets uw ON uwd.UserWalletId=uw.Id 
                                  JOIN UserWithdrawalFee uwf ON uwf.WithdrawalId=uwd.Id
                                 WHERE uwd.UserAccountId=@UserAccountId");
                    break;
                case 2:
                    sb.AppendLine(@"SELECT 2 [Type],CONVERT(VARCHAR(50),o.Id) OrderId,o.[Status],o.[Timestamp],o.[CryptoAmount],o.[CryptoId],o.[OrderNo],ma.[MerchantName] FROM [Orders] o 
                                  JOIN MerchantAccounts ma ON o.MerchantAccountId=ma.Id
                                 WHERE o.[Status]=2 AND o.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 3 [Type],CONVERT(VARCHAR(50),o.Id) OrderId,o.[Status],r.[Timestamp],o.[CryptoAmount],o.[CryptoId],o.[OrderNo],ma.[MerchantName] FROM Refunds r 
                                  JOIN Orders o ON r.OrderId = o.Id 
                                  JOIN MerchantAccounts ma ON o.MerchantAccountId=ma.Id
                                 WHERE o.[Status]=3 AND o.UserAccountId=@UserAccountId
                             UNION ALL
                                 SELECT 12 AS [Type],CONVERT(VARCHAR(50),[Id]) AS OrderId,[Status],[Timestamp],[CryptoAmount],[CryptoId],[OrderNo],[MerchantInfoName] AS MerchantName FROM [dbo].[StoreOrders] WHERE [UserAccountId]=@UserAccountId");
                    break;
                case 3:
                    sb.AppendLine(@"SELECT 3 [Type],CONVERT(VARCHAR(50),o.Id) OrderId,o.[Status],r.[Timestamp],o.[CryptoAmount],o.[CryptoId],o.[OrderNo],ma.[MerchantName] FROM Refunds r 
                                  JOIN Orders o ON r.OrderId = o.Id 
                                  JOIN MerchantAccounts ma ON o.MerchantAccountId=ma.Id
                                 WHERE o.[Status]=3 AND o.UserAccountId=@UserAccountId
                                UNION ALL
                                SELECT 10 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwr.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName] FROM [GatewayRefundOrders] gwr 
                                  JOIN GatewayOrders gwo ON gwr.OrderId = gwo.Id 
                                 WHERE gwo.[Status]=3 AND gwo.UserAccountId=@UserAccountId
                                UNION ALL
                                SELECT 11 [Type],CONVERT(VARCHAR(50),bo.Id) OrderId,bo.[Status],bo.[Timestamp],bo.[CryptoAmount],bo.[CryptoId],bo.[OrderNo],NULL [MerchantName] FROM [BillerOrders] bo
                                 WHERE bo.AccountId=@UserAccountId AND bo.[Status]=1");
                    break;
                case 4:
                    sb.AppendLine(@"SELECT 4 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp], [Amount] [CryptoAmount],[CoinId] [CryptoId],[OrderNo],NULL [MerchantName] FROM [dbo].[UserTransfers]  
                                 WHERE FromUserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 5 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp], [Amount] [CryptoAmount],[CoinId] [CryptoId],[OrderNo],NULL [MerchantName] FROM [dbo].[UserTransfers]  
                                 WHERE ToUserAccountId=@UserAccountId");
                    break;
                case 5:
                    sb.AppendLine(@"SELECT 5 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp], [Amount] [CryptoAmount],[CoinId] [CryptoId],[OrderNo],NULL [MerchantName] FROM [dbo].[UserTransfers]  
                                 WHERE ToUserAccountId=@UserAccountId");
                    break;
                case 6:
                    sb.AppendLine(@"SELECT 6 [Type],CONVERT(VARCHAR(50),uto.Id) OrderId,uto.[Status],uto.[Timestamp],uto.Amount CryptoAmount,uto.CryptoId,[OrderNo],NULL [MerchantName] FROM UserExTransferOrders uto
                                 WHERE OrderType=1 AND AccountId=@UserAccountId
                             UNION ALL
                                SELECT 7 [Type],CONVERT(VARCHAR(50),uto.Id) OrderId,uto.[Status],uto.[Timestamp],uto.Amount CryptoAmount,uto.CryptoId,[OrderNo],NULL [MerchantName] FROM UserExTransferOrders uto
                                 WHERE OrderType=2 AND AccountId=@UserAccountId");
                    break;
                case 7:
                    sb.AppendLine(@"SELECT 7 [Type],CONVERT(VARCHAR(50),uto.Id) OrderId,uto.[Status],uto.[Timestamp],uto.Amount CryptoAmount,uto.CryptoId,[OrderNo],NULL [MerchantName] FROM UserExTransferOrders uto
                                 WHERE OrderType=2 AND AccountId=@UserAccountId");
                    break;
                case 8:
                    sb.AppendLine(@"SELECT 8 [Type],CONVERT(VARCHAR(50),Id) OrderId,[Status],[Timestamp],[CryptoAmount],[CryptoId],[OrderNo],NULL [MerchantName] FROM [ProfitDetails] 
                                 WHERE AccountId=@UserAccountId");
                    break;
                case 9:
                    sb.AppendLine(@"SELECT 9 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwo.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName] FROM [GatewayOrders] gwo 
                                 WHERE gwo.[Status]=2 AND gwo.UserAccountId=@UserAccountId
                             UNION ALL
                                SELECT 10 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwr.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName] FROM [GatewayRefundOrders] gwr 
                                  JOIN GatewayOrders gwo ON gwr.OrderId = gwo.Id 
                                 WHERE gwo.[Status]=3 AND gwo.UserAccountId=@UserAccountId");
                    break;
                case 10:
                    sb.AppendLine(@"SELECT 10 [Type],CONVERT(VARCHAR(50),gwo.Id) OrderId,gwo.[Status],gwr.[Timestamp],gwo.[CryptoAmount],gwo.[CryptoId],gwo.[OrderNo],gwo.[MerchantName] FROM [GatewayRefundOrders] gwr 
                                  JOIN GatewayOrders gwo ON gwr.OrderId = gwo.Id 
                                 WHERE gwo.[Status]=3 AND gwo.UserAccountId=@UserAccountId");
                    break;
                case 11:
                    sb.AppendLine(@"SELECT 11 [Type],CONVERT(VARCHAR(50),bo.Id) OrderId,bo.[Status],bo.[Timestamp],bo.[CryptoAmount],bo.[CryptoId],bo.[OrderNo],NULL [MerchantName] FROM [BillerOrders] bo
                                 WHERE bo.AccountId=@UserAccountId");
                    break;
                case 13:
                    sb.AppendLine(@"SELECT 13 AS [Type],CONVERT(VARCHAR(50),a.[Id]) AS OrderId,a.[Status],a.[Timestamp],a.[CryptoActualAmount] AS CryptoAmount,a.[CryptoId],a.[OrderNo],a.[MerchantInfoName] AS MerchantName FROM [dbo].[StoreOrders] a
                    LEFT JOIN [dbo].[MerchantInformations] b ON a.[MerchantInfoId]=b.Id WHERE b.[AccountId]=@UserAccountId");
                    break;
                case 14:
                    sb.AppendLine(@"SELECT 14 AS [Type],CONVERT(VARCHAR(50),[Id]) AS OrderId,[Status],[Timestamp],[Amount] AS CryptoAmount,[CryptoId],
                                    (CASE WHEN [Status]> 1 THEN(CASE WHEN[Balance] = 0 THEN 0 WHEN[Amount] >[Balance] THEN 1 WHEN[Amount] =[Balance] THEN 2 ELSE 0 END) ELSE 0 END) RefundStatus
                                    FROM [dbo].[RedPockets] WHERE [AccountId]=@UserAccountId
                                    UNION ALL
                                    SELECT 15 AS [Type],CONVERT(VARCHAR(50),a.[Id]) AS OrderId,a.[Status],a.[Timestamp],b.[Amount] AS CryptoAmount,a.[CryptoId],0 [RefundStatus] FROM [dbo].[RedPockets] a
                                     LEFT JOIN [dbo].[RedPocketReceivers] b ON a.Id=b.PocketId
                                     WHERE b.[AccountId]=@UserAccountId");
                    break;
            }
            sb.AppendLine(") a  WHERE 1=1");
            sb.AppendLine($" AND [Type]<={maxType}");
            if (!string.IsNullOrEmpty(keyword))
            {
                sb.AppendLine("AND ([MerchantName] LIKE @keyword OR [OrderNo] LIKE @keyword)");
            }
            if (coinId.HasValue)
            {
                sb.AppendLine($"AND [CryptoId] = {coinId}");
            }
            if (!string.IsNullOrEmpty(mounth))
            {
                var arr = mounth.Split('-');
                var yy = int.Parse(arr[0]);
                var mm = int.Parse(arr[1]);
                sb.AppendLine($"AND DATEPART(YY,[Timestamp]) = {yy}");
                sb.AppendLine($"AND DATEPART(MM,[Timestamp]) = {mm}");
            }
            else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                if (endDate.Length == 10)
                {
                    endDate += " 23:59:59";
                }
                sb.AppendLine("AND [Timestamp] > @startDate");
                sb.AppendLine("AND [Timestamp] <= @endDate");
            }

            sb.AppendLine("ORDER BY [Timestamp] DESC");
            sb.AppendLine($"OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            var sql = sb.ToString();

            using (var con = await ReadConnectionAsync())
            {
                var list = await con.QueryAsync<UserWalletStatementES>(sql, new { UserAccountId, startDate, endDate, keyword = string.IsNullOrEmpty(keyword) ? null : $"%{keyword}%" });
                return list.AsList();
            }
        }
    }
}