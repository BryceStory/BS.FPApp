using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace FiiiPay.Framework.MongoDB
{
    /// <summary>
    /// Class FiiiPay.Framework.MongoDB.MongoDBHelper
    /// </summary>
    public class MongoDBHelper
    {
        private static readonly string _dataBase = "fiiipay";
        private static readonly IMongoDatabase db;

        static MongoDBHelper()
        {
            try
            {
                var _conn = ConfigurationManager.AppSettings.Get("MongoDBConnectionString");
                if (string.IsNullOrWhiteSpace(_conn)) throw new NullReferenceException("AppSettings MongoDBConnectionString");
                var Client = new MongoClient(_conn);
                db = Client.GetDatabase(_dataBase);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region 添加

        /// <summary>
        /// Adds the signle object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool AddSignleObject<T>(T model)
        {
            try
            {
                var collection = db.GetCollection<T>(typeof(T).Name);
                collection.InsertOne(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Adds the many object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static bool AddManyObject<T>(List<T> list)
        {
            try
            {
                var collection = db.GetCollection<T>(typeof(T).Name);
                collection.InsertMany(list);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Adds the signle object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static bool AddSignleObjectAsync<T>(T model)
        {
            try
            {
                var collection = db.GetCollection<T>(typeof(T).Name);
                collection.InsertOneAsync(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Adds the many object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static bool AddManyObjectAsync<T>(List<T> list)
        {
            try
            {
                var collection = db.GetCollection<T>(typeof(T).Name);
                collection.InsertManyAsync(list);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region 删除

        /// <summary>
        /// Deletes the index of the single.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static int DeleteSingleIndex<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return (int)collection.DeleteOne(filter).DeletedCount;
        }

        /// <summary>
        /// Deletes the single index asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static int DeleteSingleIndexAsync<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return (int)collection.DeleteOneAsync(filter).Result.DeletedCount;
        }

        /// <summary>
        /// 多个删除
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        public static void DeleteMany<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            collection.DeleteMany(filter);
        }
        #endregion

        #region 更新

        /// <summary>
        /// Updates the single.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static int UpdateSingle<T>(Expression<Func<T, bool>> filter, string name, string parameter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var set = Builders<T>.Update.Set(name, parameter);
            return (int)collection.UpdateOne(filter, set).ModifiedCount;
        }

        /// <summary>
        /// Updates the one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static int UpdateOne<T>(Expression<Func<T, bool>> filter, Dictionary<string, object> parameter)
        {
            if (parameter.Count == 0) throw new ArgumentNullException();

            var collection = db.GetCollection<T>(typeof(T).Name);
            var list = new List<UpdateDefinition<T>>();
            foreach (var item in parameter)
            {
                var set = Builders<T>.Update.Set(item.Key, item.Value);
                list.Add(set);
            }
            var builders = Builders<T>.Update.Combine(list);
            return (int)collection.UpdateOne(filter, builders).ModifiedCount;
        }

        /// <summary>
        /// Updates the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="model">The model.</param>
        /// <param name="property">The property.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <returns></returns>
        public static int UpdateMany<T>(Expression<Func<T, bool>> filter, T model, List<string> property = null, bool replace = false)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var type = model.GetType();
            //修改集合
            var list = new List<UpdateDefinition<T>>();
            foreach (var propert in type.GetProperties())
            {
                if (propert.Name.ToLower() != "id")
                {
                    try
                    {
                        if (property != null && (property.Count < 1 || property.Any(o => string.Equals(o, propert.Name, StringComparison.CurrentCultureIgnoreCase))))
                        {
                            var replaceValue = propert.GetValue(model);
                            if (replaceValue != null)
                                list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                            else if (replace)
                                list.Add(Builders<T>.Update.Set(propert.Name, (object)null));
                        }
                    }
                    catch (Exception)
                    {
                        if (property == null)
                        {
                            var replaceValue = propert.GetValue(model);
                            if (replaceValue != null)
                                list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                            else if (replace)
                                list.Add(Builders<T>.Update.Set(propert.Name, (object)null));
                        }
                    }

                }
            }
            if (list.Count > 0)
            {
                var builders = Builders<T>.Update.Combine(list);
                return (int)collection.UpdateMany(filter, builders).ModifiedCount;
            }
            return 0;
        }

        /// <summary>
        /// Updates the single asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static int UpdateSingleAsync<T>(Expression<Func<T, bool>> filter, string name, string parameter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var set = Builders<T>.Update.Set(name, parameter);
            return (int)collection.UpdateOneAsync(filter, set).Result.ModifiedCount;
        }

        /// <summary>
        /// Updates the many asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="Root">The root.</param>
        /// <param name="property">The property.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <returns></returns>
        public static int UpdateManyAsync<T>(Expression<Func<T, bool>> filter, T Root, List<string> property = null, bool replace = false)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var type = Root.GetType();
            //修改集合
            var list = new List<UpdateDefinition<T>>();
            foreach (var propert in type.GetProperties())
            {
                if (propert.Name.ToLower() != "id")
                {
                    try
                    {
                        if (property != null && (property.Count < 1 || property.Any(o => o.ToLower() == propert.Name.ToLower())))
                        {
                            var replaceValue = propert.GetValue(Root);
                            if (replaceValue != null)
                                list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                            else if (replace)
                                list.Add(Builders<T>.Update.Set(propert.Name, (object)null));
                        }
                    }
                    catch (Exception)
                    {
                        if (property == null)
                        {
                            var replaceValue = propert.GetValue(Root);
                            if (replaceValue != null)
                                list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                            else if (replace)
                                list.Add(Builders<T>.Update.Set(propert.Name, (object)null));
                        }
                    }

                }
            }
            if (list.Count > 0)
            {
                var builders = Builders<T>.Update.Combine(list);
                return (int)collection.UpdateOneAsync(filter, builders).Result.ModifiedCount;
            }
            return 0;
        }

        public static int ReplaceOne<T>(Expression<Func<T, bool>> filter, T model)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return (int)collection.ReplaceOne(filter, model).ModifiedCount;
        }

        #endregion

        #region 查询

        /// <summary>
        /// Finds the index of the single.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static T FindSingleIndex<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Finds the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public static List<T> FindMany<T>(Expression<Func<T, bool>> filter, int pageIndex = 1, int pageSize = 20) where T : Messages
        {
            pageIndex = pageIndex <= 0 ? 0 : pageIndex; //最小页为0
            var iSkip = pageSize * pageIndex;
            var iLimit = pageSize;

            var collection = db.GetCollection<T>(typeof(T).Name);
            var sort = Builders<T>.Sort.Descending(m => m.CreateTime);
            return collection.Find(filter).Skip(iSkip).Limit(iLimit).Sort(sort).ToList();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static long GetCount<T>(Expression<Func<T, bool>> filter) where T : Messages
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.CountDocuments(filter);
        }

        /// <summary>
        /// Finds the single index asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static T FindSingleIndexAsync<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.FindAsync(filter).Result.FirstOrDefault();
        }

        /// <summary>
        /// Finds the many asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static List<T> FindManyAsync<T>(Expression<Func<T, bool>> filter)
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.FindAsync(filter).Result.ToList();
        }

        #endregion
    }
}
