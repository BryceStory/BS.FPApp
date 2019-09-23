using FiiiPay.Foundation.Entities.Enum;
using System;

namespace FiiiPay.Foundation.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_CN { get; set; }
        public string PhoneCode { get; set; }
        
        public string PinYin { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 国家地区码，比如：CN、MY、HK、TW
        /// </summary>
        public string Code { get; set; }

        public CountryStatus Status { get; set; }
        public Guid NationalFlagURL { get; set; }

        public bool IsSupportStore { get; set; }
    }
}
