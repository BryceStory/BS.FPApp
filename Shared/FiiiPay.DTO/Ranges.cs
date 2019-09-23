namespace FiiiPay.Framework.Constants
{
    public class Ranges
    {
        public const int MinCountryId = 0;
        public const int MaxCountryId = int.MaxValue;

        public const int MinCoinId = 0;
        public const int MaxCoinId = int.MaxValue;

        public const string MinCoinAmount = "0";
        public const string MaxCoinAmount = "9999999999";

        public const string MinFiatAmount = "0.01";
        public const string MaxFiatAmount = "99999999999999";
    }
}