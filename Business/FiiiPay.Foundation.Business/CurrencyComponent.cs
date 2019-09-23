using System.Collections.Generic;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Business
{
    /// <summary>
    /// 法币业务类
    /// </summary>
    public class CurrencyComponent
    {
        public List<Currencies> GetList(bool isZH)
        {
            var currencyList = new CurrenciesDAC().GetAll();

            var list = new List<Currencies>();

            if (currencyList == null || currencyList.Count <= 0)
                return new List<Currencies>();

            foreach (var item in currencyList)
            {
                list.Add(new Currencies
                {
                    ID = item.ID,
                    Name = isZH ? item.Name_CN : item.Name,
                    Code = item.Code
                });
            }

            return list;
        }
    }
}
