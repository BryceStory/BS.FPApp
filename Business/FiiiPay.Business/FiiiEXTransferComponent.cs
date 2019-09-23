using System;
using FiiiPay.Business.FiiiExService;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;

namespace FiiiPay.Business
{
    public partial class FiiiEXTransferComponent
    {
        private decimal FiiiExBalance(FiiiType fiiiType, Guid accountId, Cryptocurrency crypto)
        {
            var openAccountDac = new OpenAccountDAC();
            var data = openAccountDac.GetOpenAccount(fiiiType, accountId);
            if (data == null)
                return 0M;

            LogHelper.Info($"GetFiiiExUserCoin: openId={data.OpenId}, coinCode={crypto.Code}");
            int result = new TransferCoinSoapClient().GetUserCoin(data.OpenId.ToString(), crypto.Code, out decimal amount);
            LogHelper.Info($"GetFiiiExUserCoin: result={result}, amount={amount}");
            return amount;
        }

        private int FiiiExCoinIn(Guid openId, string coinCode, decimal amount, out string recordId)
        {
            LogHelper.Info($"FiiiExCoinIn: openId={openId}, coinCode={coinCode}, amount={amount}");
            int result = new TransferCoinSoapClient().CoinIn(openId.ToString(), coinCode, amount, out recordId);
            LogHelper.Info($"FiiiExCoinIn: result={result}, recordId={recordId}");
            return result;
        }

        private int FiiiExCoinOut(Guid openId, string coinCode, decimal amount, out string recordId)
        {
            LogHelper.Info($"FiiiExCoinOut: openId={openId}, coinCode={coinCode}, amount={amount}");
            int result = new TransferCoinSoapClient().CoinOut(openId.ToString(), coinCode, amount, out recordId);
            LogHelper.Info($"FiiiExCoinOut: result={result}, recordId={recordId}");
            return result;
        }


        private string CreateOrderNo()
        {
            return DateTime.Now.ToUnixTime() + new Random().Next(0, 100).ToString("00");
        }


        public class TransferResult
        {
            public long Id { get; set; }
            public string OrderNo { get; set; }
            public long Timestamp { get; set; }
            public string Amount { get; set; }
            public string CryptoCode { get; set; }
        }

        #region FiiiEx Web Services

        public string TransferInto(Guid openId, string pin, string coinCode, decimal amount)
        {
            var coin = new CryptocurrencyDAC().GetByCode(coinCode);
            if (coin == null)
                throw new CommonException(ReasonCode.CRYPTO_NOT_EXISTS, R.ErrorCryptoCode);

            var openAccountDac = new OpenAccountDAC();
            var openAccount = openAccountDac.GetOpenAccount(openId);
            if (openAccount == null)
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.AccountNotExist);
            switch (openAccount.FiiiType)
            {
                case FiiiType.FiiiPay:
                    var accountDac = new UserAccountDAC();
                    var userAccount = accountDac.GetById(openAccount.AccountId);
                    new SecurityComponent().VerifyPin(userAccount, pin);
                    return this.FiiiPayTransferInto(userAccount, coin, amount);
                case FiiiType.FiiiPOS:
                    MerchantAccount merchantAccount = new MerchantAccountDAC().GetById(openAccount.AccountId);
                    new SecurityComponent().FiiiPOSVerifyPin(merchantAccount, pin);
                    return this.FiiiPOSTransferInto(merchantAccount, coin, amount);
                default:
                    throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.AccountNotExist);
            }
        }
        //public bool VerifyPIN(Guid openId, string pin)
        //{
        //    var openAccountDac = new OpenAccountDAC();
        //    var openAccount = openAccountDac.GetOpenAccount(openId);
        //    if (openAccount == null)
        //        throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.AccountNotExist);
        //    switch (openAccount.FiiiType)
        //    {
        //        case FiiiType.FiiiPay:
        //            UserAccount userAccount = new UserAccountDAC().GetById(openAccount.AccountId);
        //            new SecurityComponent().VerifyPin(userAccount, pin);
        //            return true;
        //        case FiiiType.FiiiPOS:
        //            MerchantAccount merchantAccount = new MerchantAccountDAC().GetById(openAccount.AccountId);
        //            new SecurityComponent().FiiiPOSVerifyPin(merchantAccount, pin);
        //            return true;
        //        default:
        //            throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, R.AccountNotExist);
        //    }
        //}
        #endregion

    }
}