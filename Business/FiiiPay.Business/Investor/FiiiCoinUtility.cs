using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Business
{
    public static class FiiiCoinUtility
    {
        private const string Code = "FIII";

        private static Cryptocurrency _cryptocurrency;

        public static Cryptocurrency Cryptocurrency => _cryptocurrency ?? (_cryptocurrency = new CryptocurrencyDAC().GetByCode(Code));
    }
}