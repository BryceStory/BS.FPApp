using StackExchange.Redis;

namespace FiiiPay.Framework.Cache
{
    internal class DatabaseModel
    {
        public IDatabase DefaultDatabase { get; }

        public IServer Server { get; }

        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public DatabaseModel(IDatabase defaultDatabase, IServer server, ConnectionMultiplexer connectionMultiplexer)
        {
            DefaultDatabase = defaultDatabase;
            Server = server;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IDatabase GetDatabase(int db)
        {
            return _connectionMultiplexer.GetDatabase(db);
        }
    }
}
