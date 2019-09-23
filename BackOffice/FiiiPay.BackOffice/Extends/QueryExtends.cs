using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FiiiPay.BackOffice
{
    public static class QueryExtends
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, String propertyName, String sortName)
        {
            string MethodName = "";
            if (string.IsNullOrEmpty(sortName))
                MethodName = "OrderBy";
            else if (sortName.ToUpper() == "ASC")
                MethodName = "OrderBy";
            else if (sortName.ToUpper() == "DESC")
                MethodName = "OrderByDescending";
            return InternalOrder<T>(source, propertyName, MethodName);
        }
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenByDescending");
        }

        private static IOrderedQueryable<T> InternalOrder<T>(IQueryable<T> source, String propertyName, String methodName)
        {
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "p");
            System.Reflection.PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("propertyName", "Not Exist");
            Expression expr = Expression.Property(arg, property);
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            return ((IOrderedQueryable<T>)(typeof(Queryable).GetMethods().Single(
                p => String.Equals(p.Name, methodName, StringComparison.Ordinal)
                    && p.IsGenericMethodDefinition
                    && p.GetGenericArguments().Length == 2
                    && p.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType)
                .Invoke(null, new Object[] { source, lambda })));
        }

        public static IList<T> OrderBy<T>(this IList<T> source, String propertyName, String sortName)
        {
            string MethodName = "";
            if (string.IsNullOrEmpty(sortName))
                MethodName = "OrderBy";
            else if (sortName.ToUpper() == "ASC")
                MethodName = "OrderBy";
            else if (sortName.ToUpper() == "DESC")
                MethodName = "OrderByDescending";
            return InternalOrder<T>(source, propertyName, MethodName);
        }
        public static IList<T> OrderBy<T>(this IList<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderBy");
        }

        public static IList<T> OrderByDescending<T>(this IList<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderByDescending");
        }

        public static IList<T> ThenBy<T>(this IList<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenBy");
        }
        public static IList<T> ThenByDescending<T>(this IList<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenByDescending");
        }

        private static IList<T> InternalOrder<T>(IList<T> source, String propertyName, String methodName)
        {
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "p");
            System.Reflection.PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("propertyName", "Not Exist");
            Expression expr = Expression.Property(arg, property);
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            var mlist = typeof(List<T>).GetMethods();
            return ((IQueryable<T>)(typeof(Queryable).GetMethods().Single(
                p => String.Equals(p.Name, methodName, StringComparison.Ordinal)
                    && p.IsGenericMethodDefinition
                    && p.GetGenericArguments().Length == 2
                    && p.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType)
                .Invoke(null, new Object[] { source.AsQueryable(), lambda }))).ToList();
        }
    }
}