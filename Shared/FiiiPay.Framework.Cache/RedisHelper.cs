using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FiiiPay.Framework.Cache
{
    /// <summary>
    /// Class FiiiPay.Framework.Cache.RedisHelper
    /// </summary>
    public class RedisHelper
    {
        private static readonly Lazy<DatabaseModel> DatabaseConnection = new Lazy<DatabaseModel>(ConnectionRedis);
        private static IDatabase _writeDatabase => DatabaseConnection.Value.DefaultDatabase;
        private static DatabaseModel _database => DatabaseConnection.Value;
        private static IServer _readServer => DatabaseConnection.Value.Server;

        /// <summary>
        /// The default redis connection key
        /// </summary>
        public const string DefaultRedisConnectionKey = "RedisConnectionString";
        /// <summary>
        /// The default default database key
        /// </summary>
        public const string DefaultDefaultDatabaseKey = "RedisDefaultDatabase";
        /// <summary>
        /// The default redis server key
        /// </summary>
        public const string DefaultRedisServerKey = "RedisServer";

        /// <summary>
        /// Strings the get.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string StringGet(string key)
        {
            var index = 0;
            var result = "";
            do
            {
                try
                {
                    result = _writeDatabase.StringGet(key);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Strings the get.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string StringGet(int db, string key)
        {
            var index = 0;
            var result = "";
            do
            {
                try
                {
                    result = _database.GetDatabase(db).StringGet(key);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            string str = _writeDatabase.StringGet(key);
            if (str == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// Gets the specified database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(int database, string key)
        {
            string value = _database.GetDatabase(database).StringGet(key);
            if (value == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static long GetSize(int database)
        {
            return _readServer.DatabaseSize(database);
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static List<string> GetKeys(int database, string key)
        {
            var list = _readServer.Keys(database, key + "*");
            var redisKeys = list as RedisKey[] ?? list.ToArray();
            if (!redisKeys.Any()) return new List<string>();

            return redisKeys.Select(s => s.ToString()).ToList();
        }

        /// <summary>
        /// Strings the set.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool StringSet(string key, string value, TimeSpan? expiry = null)
        {
            var index = 0;
            bool result = false;
            do
            {
                try
                {
                    result = _writeDatabase.StringSet(key, value, expiry);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool Set(string key, object value, TimeSpan? expiry = null)
        {
            return _writeDatabase.StringSet(key, JsonConvert.SerializeObject(value), expiry);
        }

        /// <summary>
        /// Sets the specified database.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool Set(int db, string key, object value, TimeSpan? expiry = null)
        {
            return _database.GetDatabase(db).StringSet(key, JsonConvert.SerializeObject(value), expiry);
        }

        /// <summary>
        /// Strings the set.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool StringSet(int db, string key, string value, TimeSpan? expiry = null)
        {
            var index = 0;
            bool result = false;
            do
            {
                try
                {
                    result = _database.GetDatabase(db).StringSet(key, value, expiry);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Keys the delete.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            var index = 0;
            bool result = false;
            do
            {
                try
                {
                    result = _writeDatabase.KeyDelete(key);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Keys the delete.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool KeyDelete(int db, string key)
        {
            var index = 0;
            bool result = false;
            do
            {
                try
                {
                    result = _database.GetDatabase(db).KeyDelete(key);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        /// <summary>
        /// Keys the expire.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool KeyExpire(string key, TimeSpan expiry)
        {
            return _writeDatabase.KeyExpire(key, expiry);
        }

        /// <summary>
        /// Keys the expire.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public static bool KeyExpire(int db, string key, TimeSpan expiry)
        {
            return _database.GetDatabase(db).KeyExpire(key, expiry);
        }

        /// <summary>
        /// Keys the exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool KeyExists(string key)
        {
            return _writeDatabase.KeyExists(key);
        }

        /// <summary>
        /// Keys the exists.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool KeyExists(int db, string key)
        {
            return _database.GetDatabase(db).KeyExists(key);
        }

        /// <summary>
        /// Keys the time to live.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static TimeSpan? KeyTimeToLive(string key)
        {
            return _writeDatabase.KeyTimeToLive(key);
        }

        /// <summary>
        /// Keys the time to live.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static TimeSpan? KeyTimeToLive(int db, string key)
        {
            return _database.GetDatabase(db).KeyTimeToLive(key);
        }

        /// <summary>
        /// Lists the right push.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static long ListRightPush(int db,string key,object value)
        {
            return _database.GetDatabase(db).ListRightPush(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Lists the left pop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">The database.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T ListLeftPop<T>(int db, string key)
        {
            var str = _database.GetDatabase(db).ListLeftPop(key);
            if (str.IsNull)
                return default(T);
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <returns></returns>
        public static IDatabase GetDatabase(int db)
        {
            var index = 0;
            IDatabase result = null;
            do
            {
                try
                {
                    result = _database.GetDatabase(db);
                    break;
                }
                catch
                {
                    index++;
                    Thread.Sleep(300);
                }
            } while (index < 5);

            return result;
        }

        private static DatabaseModel ConnectionRedis()
        {
            var connection = ConfigurationManager.AppSettings.Get(DefaultRedisConnectionKey);
            var RedisDefaultDatabaseSetting = ConfigurationManager.AppSettings.Get(DefaultDefaultDatabaseKey);
            var hostAndPort = ConfigurationManager.AppSettings.Get(DefaultRedisServerKey);

            var connectionMultiplexer = ConnectionMultiplexer.Connect(connection);
            var dbIndex = string.IsNullOrWhiteSpace(RedisDefaultDatabaseSetting)
                ? 0
                : int.Parse(RedisDefaultDatabaseSetting);

            var db = connectionMultiplexer.GetDatabase(dbIndex);

            var server = !string.IsNullOrWhiteSpace(hostAndPort)
                ? connectionMultiplexer.GetServer(hostAndPort)
                : null;

            return new DatabaseModel(db, server, connectionMultiplexer);
        }
    }
}
