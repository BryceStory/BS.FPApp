using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    public class EntityHelper
    {
        public static List<PropertyInfo> GetPropertyInfoList<T>(DataColumnCollection columns) where T : new()
        {
            List<PropertyInfo> propertyList = new List<PropertyInfo>();

            if (columns != null)
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties<T>();

                foreach (var item in properties)
                {
                    if (columns.Contains(item.Name))
                    {
                        propertyList.Add(item);
                    }
                }
            }

            return propertyList;
        }

        public static T SetPropertyValue<T>(DataRow dr, List<PropertyInfo> propertyList) where T : new()
        {
            T t = new T();
            foreach (var pro in propertyList)
            {
                if (dr[pro.Name] == DBNull.Value)
                {
                    if (pro.PropertyType.IsClass || pro.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        pro.SetValue(t, null, null);
                    }
                    else
                    {
                        pro.SetValue(t, default(T), null);
                    };
                    continue;
                }

                Type dbType = dr[pro.Name].GetType();
                switch (dbType.Name)
                {
                    case "Guid":
                        pro.SetValue(t, Guid.Parse(dr[pro.Name].ToString()), null);
                        break;
                    case "Int64":
                        if (pro.PropertyType == typeof(Int32) || pro.PropertyType == typeof(Nullable<Int32>))
                        {
                            pro.SetValue(t, Convert.ToInt32(dr[pro.Name].ToString()), null);
                        }
                        else
                        {
                            pro.SetValue(t, dr[pro.Name], null);
                        }
                        break;
                    default:
                        if (pro.PropertyType == typeof(Nullable<Int32>) || pro.PropertyType == typeof(Nullable<Int64>))
                        {
                            Type realType = Nullable.GetUnderlyingType(pro.PropertyType) ?? pro.PropertyType;
                            object safeValue = Convert.ChangeType(dr[pro.Name], realType);
                            pro.SetValue(t, safeValue, null);
                        }
                        else
                        {
                            SetValue(t, pro, dr[pro.Name]);
                        }
                        break;
                };

            }
            return t;
        }

        private static void SetValue<T>(T t, PropertyInfo pro, object value) where T : new()
        {
            try
            {
                pro.SetValue(t, value, null);
            }
            catch
            {
                if (pro.PropertyType.FullName.Contains("Decimal"))
                {
                    if (pro.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        #region
                        decimal? newValue;
                        if (value == null)
                        {
                            newValue = null;
                        }
                        else
                        {
                            //newValue = decimal.Parse(value.ToString());
                            decimal realValue;
                            if (!decimal.TryParse(value.ToString(), out realValue))
                            {

                                if (!decimal.TryParse(value.ToString(), System.Globalization.NumberStyles.Float, null, out realValue))
                                {
                                    throw new InvalidCastException(string.Format("Trying phase {0} to decimal failed!!", value.ToString()));
                                }
                                else
                                {
                                    newValue = realValue;
                                }
                            }
                            else
                            {
                                newValue = realValue;
                            }
                        }
                        #endregion
                        pro.SetValue(t, newValue, null);
                    }
                    else
                    {
                        decimal newValue = decimal.Parse(value.ToString());
                        pro.SetValue(t, newValue, null);
                    }
                }
                if (pro.PropertyType.FullName.Contains("Byte"))
                {
                    if (pro.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        #region
                        byte? newValue;
                        if (value == null)
                        {
                            newValue = null;
                        }
                        else
                        {
                            byte realValue;
                            if (!byte.TryParse(value.ToString(), out realValue))
                            {

                                if (!byte.TryParse(value.ToString(), System.Globalization.NumberStyles.Integer, null, out realValue))
                                {
                                    throw new InvalidCastException(string.Format("Trying phase {0} to decimal failed!!", value.ToString()));
                                }
                                else
                                {
                                    newValue = realValue;
                                }
                            }
                            else
                            {
                                newValue = realValue;
                            }
                        }
                        #endregion
                        pro.SetValue(t, newValue, null);
                    }
                    else
                    {
                        byte newValue = byte.Parse(value.ToString());
                        pro.SetValue(t, newValue, null);
                    }
                }
                else if (pro.PropertyType.FullName.Contains("DateTime"))
                {
                    //log.Exception("see see what is format");
                    //log.Exception(value.ToString());
                    DateTimeFormatInfo format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                    if (pro.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        #region
                        DateTime? newValue;
                        if (value == null)
                        {
                            newValue = null;
                        }
                        else
                        {
                            newValue = DateTime.ParseExact(value.ToString(), string.Format("{0} {1}", format.ShortDatePattern, format.LongTimePattern), format);
                        }
                        #endregion
                        pro.SetValue(t, newValue, null);
                    }
                    else
                    {
                        DateTime newValue = DateTime.ParseExact(value.ToString(), string.Format("{0} {1}", format.ShortDatePattern, format.LongTimePattern), format);
                        pro.SetValue(t, newValue, null);
                    }
                }
                else if (pro.PropertyType.IsEnum)
                {
                    var enumValue = Enum.Parse(pro.PropertyType, value.ToString());
                    pro.SetValue(t, enumValue, null);
                }
                else
                {
                    throw new InvalidCastException(
                        string.Format("Trying phase EntityType[{0}] to DbType[{1}] failed!!", pro.PropertyType.FullName, value.GetType().FullName));
                }
            }
        }

        public static List<T> EncapsulateObj<T>(DataSet ds) where T : new()
        {
            List<T> result = new List<T>();
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                List<PropertyInfo> propertyList = GetPropertyInfoList<T>(dt.Columns);

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(SetPropertyValue<T>(row, propertyList));
                }
            }
            return result;
        }

        public static List<T> EncapsulateObj<T>(DataRow[] drs) where T : new()
        {
            List<T> result = new List<T>();
            foreach (DataRow dr in drs)
            {
                result.Add(EncapsulateObj<T>(dr));
            }
            return result;
        }

        public static T EncapsulateObj<T>(DataRow dr) where T : new()
        {
            List<PropertyInfo> propertyList = GetPropertyInfoList<T>(dr.Table.Columns);
            return SetPropertyValue<T>(dr, propertyList);
        }

        public static T CopyObject<T>(T t) where T : new()
        {
            T ret = new T();
            PropertyInfo pi = null;
            PropertyInfo[] propertyList = t.GetType().GetProperties();
            for (int i = 0; i < propertyList.Length; i++)
            {
                pi = propertyList[i];
                SetValue(ret, propertyList[i], pi.GetValue(t, null));
            }
            return ret;
        }

        public static T EncapsulateBasicObj<T>(DataRow dr)
        {
            if (dr.Table.Columns.Count == 1)
                return (T)dr[0];
            else
                return default(T);
        }

        public static List<T> EncapsulateObj<T>(DataTable dt) where T : new()
        {
            List<T> result = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                List<PropertyInfo> propertyList = GetPropertyInfoList<T>(dt.Columns);

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(SetPropertyValue<T>(row, propertyList));
                }
            }
            return result;
        }
    }
}