using FiiiPay.BackOffice.Models;
using System.Collections.Generic;

namespace FiiiPay.BackOffice.BLL
{
    public class AreaBLL : BaseBLL
    {
        public List<Countries> GetCountrysByIds(List<int> ids)
        {
            if (ids == null || ids.Count <= 0)
                return new List<Countries>();
            return FoundationDB.CountryDb.GetList(t => ids.Contains(t.Id));
        }

        public List<States> GetStatesByCountryIds(List<int> ids)
        {
            if (ids == null || ids.Count <= 0)
                return new List<States>();
            return FoundationDB.StateDb.GetList(t => ids.Contains(t.CountryId));
        }

        public List<Cities> GetCitysByStateIds(List<int> ids)
        {
            if (ids == null || ids.Count <= 0)
                return new List<Cities>();
            return FoundationDB.CityDb.GetList(t => ids.Contains(t.StateId));
        }

        public List<States> GetStatesByCountryId(int countryId)
        {
            return FoundationDB.StateDb.GetList(t => t.CountryId == countryId);
        }

        public List<Cities> GetCitysByStateId(int stateId)
        {
            return FoundationDB.CityDb.GetList(t => t.StateId == stateId);
        }
    }
}