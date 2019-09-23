using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Common
{
    public class EnumEntity
    {
        public string Desction { set; get; }
        
        public string EnumName { set; get; }
        
        public int EnumValue { set; get; }

        public bool Ischecked { set; get; }
    }
    public class EnumHelper
    {
        public static List<EnumEntity> EnumToList<T>()
        {
            List<EnumEntity> list = new List<EnumEntity>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumEntity m = new EnumEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }

        public static List<SelectListItem> GetEnumSelectList<T>(string v = "", bool addAll = false) 
        {
            var type = typeof(T);
            var enumNames = Enum.GetNames(type);
            var enumValues = Enum.GetValues(type);
            List<SelectListItem> oList = new List<SelectListItem>();
            if (addAll)
            {
                oList.Add(new SelectListItem() { Text = "All", Value = "" });
            }
            foreach (var item in Enum.GetValues(type))
            {
                byte ev = Convert.ToByte(item);
                oList.Add(new SelectListItem() { Text = item.ToString(), Value = ev.ToString(), Selected = ev.ToString() == v });
            }
            return oList;
        }
    }
}