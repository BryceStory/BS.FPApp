using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class GetLastActiveCountryOM
    {
        public string Country_CN { get; set; }

        public string Country_EN { get; set; }

        public int CountryId { get; set; }

        /// <summary>
        /// 是否是第一次定位
        /// </summary>
        public bool IsFirst { get; set; }
    }

    public class SetActiveCountryIM
    {
        public string Country_CN { get; set; }

        public string Country_EN { get; set; }

        public int CountryId { get; set; }
    }
}
