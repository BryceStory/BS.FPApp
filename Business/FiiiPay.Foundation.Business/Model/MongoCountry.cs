using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework.MongoDB;

namespace FiiiPay.Foundation.Business.Model
{
    public class MongoCountry : MongoBaseEntity
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Name_CN { get; set; }
        public string PhoneCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomerService { get; set; }

        public string PinYin { get; set; }

        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 国家地区码，比如：CN、MY、HK、TW
        /// </summary>
        public string Code { get; set; }

        public bool IsHot { get; set; }

        public CountryStatus Status { get; set; }
        public Guid NationalFlagURL { get; set; }
        public string FiatCurrencySymbol { get; set; }

        public bool IsSupportStore { get; set; } = false;

        public static implicit operator Country(MongoCountry country)
        {
            if (country == null)
                return null;
            return new Country()
            {
                Id = country.CountryId,
                Code = country.Code,
                //CustomerService = country.CustomerService,
                FiatCurrency = country.FiatCurrency,
                //FiatCurrencySymbol = country.FiatCurrencySymbol,
                //IsHot = country.IsHot,
                Name = country.Name,
                NationalFlagURL = country.NationalFlagURL,
                Name_CN = country.Name_CN,
                PhoneCode = country.PhoneCode,
                PinYin = country.PinYin,
                Status = country.Status,
                IsSupportStore = country.IsSupportStore
            };

        }
    }
}