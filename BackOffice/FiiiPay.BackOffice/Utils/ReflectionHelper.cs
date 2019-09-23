using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    public static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<string, TypeCache> _cache = new ConcurrentDictionary<string, TypeCache>();

        private class TypeCache
        {
            public Type Type { get; set; }

            public PropertyInfo[] PropertyInfos { get; set; }
        }

        public static PropertyInfo[] GetProperties<T>() where T : new()
        {
            Type type = typeof(T);

            TypeCache typeCache = _cache.GetOrAdd(type.FullName, (key) =>
            {
                return new TypeCache()
                {
                    Type = type,
                    PropertyInfos = type.GetProperties((BindingFlags.Public | BindingFlags.Instance))
                };
            });

            return typeCache.PropertyInfos;
        }

        public static T GetConstructor<T>()
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    new Type[0],
                    new ParameterModifier[0]);

            if (ctor == null)
            {
                throw new NotSupportedException("No constructor without parameter");
            }

            return (T)ctor.Invoke(null);
        }

        public static object GetPropertyValue<T>(T t, string propertyName) where T : new()
        {
            var info = GetProperties<T>();
            foreach (var i in info)
            {
                if (i.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return i.GetValue(t, null);
                }
            }
            return null;
        }

        public static object GetValue<T>(this ParallelQuery<PropertyInfo> properties, T t, string propertyName)
        {
            var q = properties.Where(x => x.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            if (q.Count() > 0)
            {
                return q.First().GetValue(t, null);
            }

            return null;
        }
        public static TChild AutoCopy<TParent, TChild>(TParent parent) where TChild : TParent, new()
        {
            TChild child = new TChild();
            var ParentType = typeof(TParent);
            var Properties = ParentType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    //进行属性拷贝
                    Propertie.SetValue(child, Propertie.GetValue(parent, null), null);
                }
            }
            return child;
        }
    }
}