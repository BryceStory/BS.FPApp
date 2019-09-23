using System.Collections.Generic;
using System.Linq;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Business
{
    public class CountryComponent
    {
        public List<Country> GetList(bool isZH)
        {
            return new CountryDAC().GetList().ToList();
        }

        public Country GetById(int id)
        {
            return new CountryDAC().GetById(id);
            //Country country = GetList(false).FirstOrDefault(item => item.Id == id);
            //if (country == null)
            //{
            //    country = new CountryDAC().GetById(id);
            //    //if (country != null)
            //    //{
            //    //    MongoDBHelper.AddSignleObject<MongoCountry>(new MongoCountry()
            //    //    {
            //    //        CountryId = country.Id,
            //    //        Code = country.Code,
            //    //        CustomerService = country.CustomerService,
            //    //        FiatCurrency = country.FiatCurrency,
            //    //        FiatCurrencySymbol = country.FiatCurrencySymbol,
            //    //        IsHot = country.IsHot,
            //    //        Name = country.Name,
            //    //        NationalFlagURL = country.NationalFlagURL,
            //    //        Name_CN = country.Name_CN,
            //    //        PhoneCode = country.PhoneCode,
            //    //        PinYin = country.PinYin,
            //    //        Status = country.Status
            //    //    });
            //    //}
            //}
            //return country;
        }

        /// <summary>
        /// 获取电话客服列表
        /// 若需要获取完整客服列表，还需要查询Mastersetting表获取相关数据
        /// </summary>
        /// <param name="isZH"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetCustomerService(bool isZH)
        {
            var dic = new Dictionary<string, List<string>>();
            //var countries = GetList(isZH);

            //var strList = countries.Select(country => $"{country.CustomerService}({(isZH ? country.Name_CN : country.Name)})").ToList();

            //dic.Add("Phone", strList);

            var CustomerServices = new MasterSettingDAC().SelectByGroup("CustomerService");
            if (CustomerServices != null)
            {
                foreach (var masterSetting in CustomerServices)
                {
                    if (!dic.ContainsKey(masterSetting.Name))
                    {
                        dic.Add(masterSetting.Name, new List<string> { masterSetting.Value });
                    }
                    else
                    {
                        dic[masterSetting.Name].Add(masterSetting.Value);
                    }
                }
            }

            return dic;
        }


        //public void DeleteMongoDbById(int id)
        //{
        //    MongoDBHelper.DeleteSingleIndex<MongoCountry>(item => item.CountryId == id);
        //}

        //public void InsertMongoDb(Country country)
        //{
        //    MongoDBHelper.AddSignleObject(new MongoCountry()
        //    {
        //        CountryId = country.Id,
        //        Code = country.Code,
        //        CustomerService = country.CustomerService,
        //        FiatCurrency = country.FiatCurrency,
        //        FiatCurrencySymbol = country.FiatCurrencySymbol,
        //        IsHot = country.IsHot,
        //        Name = country.Name,
        //        NationalFlagURL = country.NationalFlagURL,
        //        Name_CN = country.Name_CN,
        //        PhoneCode = country.PhoneCode,
        //        PinYin = country.PinYin,
        //        Status = country.Status
        //    });
        //}

        //public void UpdateMongoDb(Country country)
        //{
        //    var id = MongoDBHelper.FindSingleIndex<MongoCountry>(item => item.CountryId == country.Id)._id;
        //    MongoDBHelper.ReplaceOne<MongoCountry>(item => item.CountryId == country.Id, new MongoCountry()
        //    {
        //        _id = id,
        //        CountryId = country.Id,
        //        Code = country.Code,
        //        CustomerService = country.CustomerService,
        //        FiatCurrency = country.FiatCurrency,
        //        FiatCurrencySymbol = country.FiatCurrencySymbol,
        //        IsHot = country.IsHot,
        //        Name = country.Name,
        //        NationalFlagURL = country.NationalFlagURL,
        //        Name_CN = country.Name_CN,
        //        PhoneCode = country.PhoneCode,
        //        PinYin = country.PinYin,
        //        Status = country.Status
        //    });
        //}
    }
}
