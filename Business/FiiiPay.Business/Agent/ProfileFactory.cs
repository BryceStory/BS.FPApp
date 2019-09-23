using System.Collections.Generic;
using System.Linq;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Data.Agents
{
    /// <summary>
    /// 路由缓存
    /// </summary>
    public static class ProfileFactory
    {
        private static Dictionary<int, ProfileRouter> _dict;

        private static void Init()
        {
            _dict = new Dictionary<int, ProfileRouter>();
            var dac = new ProfileRouterDAC();
            var list = dac.GetAll();
            foreach (var item in list)
            {
                _dict.Add(item.Country, item);
            }
        }

        static ProfileFactory()
        {
            Init();
        }

        public static ProfileRouter GetByCountryId(int countryId)
        {
            if (_dict.Keys.Contains(countryId))
            {
                return _dict[countryId];
            }

            var dac = new ProfileRouterDAC();
            var data = dac.GetRouter(countryId);
            if (data!=null)
            {
                _dict.Add(countryId, data);
            }

            return data;
        }

        public static bool UpdateRouter(ProfileRouter router)
        {
            var dac = new ProfileRouterDAC();
            var result = dac.UpdateRouter(router);
            if (result)
            {
                Init();
            }
            return false;
        }

        public static bool DeleteRouter(int countryId)
        {
            var dac = new ProfileRouterDAC();
            var result = dac.DeleteById(countryId);
            if (result)
            {
                Init();
            }
            return false;
        }
    }
}