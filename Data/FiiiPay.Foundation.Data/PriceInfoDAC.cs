using System;
using Dapper;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.EntitySet;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.Foundation.Data
{
    public class PriceInfoDAC : BaseFoundationDataAccess
    {
        public List<PriceInfo> GetAll()
        {
            const string SQL_STATEMENT = "SELECT Price as MarketPrice,Id,CryptoID,CurrencyID,Convert(decimal(18,4),Price * Markup) as Price,LastUpdateDate FROM [dbo].[PriceInfo] ";

            using (var conn = ReadConnection())
            {
                return conn.Query<PriceInfo>(SQL_STATEMENT).ToList();
            }
        }
        
        public List<PriceInfoES> GetPrice(string currency)
        {
            string SQL_STATEMENT = @"SELECT C.Code AS Currency,CY.Code AS CryptoName,P.Price as MarketPrice,Convert(decimal(18,4),P.Price * P.Markup) as Price,CY.Id AS CryptoID,C.ID AS CurrencyID
                                    FROM PriceInfo AS P
                                    LEFT JOIN Currencies AS C ON P.CurrencyID = C.ID
                                    LEFT JOIN Cryptocurrencies AS CY ON CY.Id = P.CryptoID
                                    WHERE C.Code = @Currency";
            using (var conn = ReadConnection())
            {
                return conn.Query<PriceInfoES>(SQL_STATEMENT, new { Currency = currency }).ToList();
            }
        }

        /// <summary>
        /// Get a list of PriceInfo
        /// </summary>
        /// <returns></returns>
        /// Added by yee may 6-Apr-2018
        public List<PriceInfo> GetByCurrencyId(short currencyId)
        {
            const string SQL_STATEMENT = "SELECT Id,CryptoID,CurrencyID,Price as MarketPrice,Convert(decimal(18,4),Price * Markup) as Price,LastUpdateDate FROM [dbo].[PriceInfo] WHERE CurrencyID = @CurrencyId ";

            using (var conn = ReadConnection())
            {
                return conn.Query<PriceInfo>(SQL_STATEMENT, new { CurrencyId = currencyId }).ToList();
            }
        }
        
        public PriceInfo GetPriceInfo(int currencyId, int cryptoId)
        {
            const string SQL_STATEMENT = "SELECT Id,CryptoID,CurrencyID,Price as MarketPrice,Convert(decimal(18,4), Price * Markup) as Price,LastUpdateDate FROM [dbo].[PriceInfo] WHERE [CurrencyID] = @CurrencyId AND [CryptoID] = @CryptoId";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<PriceInfo>(SQL_STATEMENT,
                    new {CurrencyId = currencyId, CryptoId = cryptoId});
            }
        }

        public List<PriceInfoES> GetPrice(string currency, string cryptoCode)
        {
            string SQL_STATEMENT = @"SELECT C.Code AS Currency,CY.Code AS CryptoName,P.Price as MarketPrice,Convert(decimal(18,4),P.Price * P.Markup) as Price
                                    FROM PriceInfo AS P
                                    LEFT JOIN Currencies AS C ON P.CurrencyID = C.ID
                                    LEFT JOIN Cryptocurrencies AS CY ON CY.Id = P.CryptoID
                                    WHERE C.Code = @Currency AND CY.Code = @CryptoCode";
            using(var conn = ReadConnection())
            {
                return conn.Query<PriceInfoES>(SQL_STATEMENT, new { Currency = currency, CryptoCode = cryptoCode }).ToList();
            }
        }

        public decimal GetPriceByName(string currency, string cryptoCode)
        {
            string SQL_STATEMENT = @"SELECT Convert(decimal(18,4),P.Price * P.Markup) as Price
                                    FROM PriceInfo AS P
                                    LEFT JOIN Currencies AS C ON P.CurrencyID = C.ID
                                    LEFT JOIN Cryptocurrencies AS CY ON CY.Id = P.CryptoID
                                    WHERE C.Code = @Currency AND CY.Code = @CryptoCode";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<decimal>(SQL_STATEMENT, new { Currency = currency, CryptoCode = cryptoCode });
            }
        }

        public PriceInfo CreatePriceInfo(PriceInfo priceinfoitem)
        {
            const string SQL_STATEMENT =
                        "INSERT INTO dbo.[PriceInfo] ([CryptoID], [CurrencyID], [Price], [LastUpdateDate]) " +
                        "VALUES (@CryptoID, @CurrencyID, @Price, @LastUpdateDate ); SELECT SCOPE_IDENTITY(); ";

            priceinfoitem.Price = Math.Round(priceinfoitem.Price, 2);
            using (var conn = WriteConnection())
            {
                var result = conn.Execute(SQL_STATEMENT, priceinfoitem);
                if (result > 0)
                    priceinfoitem.ID = 1;
            }

            return priceinfoitem;
        }

        public bool UpdatePriceInfo(PriceInfo priceinfoitem)
        {
            const string SQL_STATEMENT =
                       "UPDATE [dbo].[PriceInfo] " +
                       "SET " +
                           "[CryptoID] = @CryptoID, " +
                           "[CurrencyID] = @CurrencyID, " +
                           "[Price] = @Price, " +
                           "[LastUpdateDate] = @LastUpdateDate " +
                       "WHERE [ID]=@ID ";
            priceinfoitem.Price = Math.Round(priceinfoitem.Price, 2);
            using (var conn = WriteConnection())
            {
                return conn.Execute(SQL_STATEMENT, priceinfoitem) > 0;
            }
        }
    }
}
