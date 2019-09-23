using System.Collections.Generic;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPOS.Business
{
    /// <summary>
    /// 法币业务类
    /// </summary>
    public class CurrencyComponent
    {
        public List<Currencies> GetList()
        {
            return new CurrenciesDAC().GetAll();
        }
    }
}
