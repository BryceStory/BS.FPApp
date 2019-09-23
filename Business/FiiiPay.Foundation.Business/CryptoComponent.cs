using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using System.Collections.Generic;

namespace FiiiPay.Foundation.Business
{
    public class CryptoComponent
    {
        public List<Cryptocurrency> GetList()
        {
            return new CryptocurrencyDAC().GetAll();
        }

        public Cryptocurrency GetByCode(string code)
        {
            return new CryptocurrencyDAC().GetByCode(code);
        }
    }
}
