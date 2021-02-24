using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Valuator.Storage
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
        private readonly IDatabase _db;
        private readonly IServer _server;

        public RedisStorage()
        {
            _db = _connection.GetDatabase();
            _server = _connection.GetServer("localhost", 6379);
        }

        public void Load(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public string GetValue(string key)
        {
            return _db.StringGet(key);
        }

        public List<string> GetValues(string prefix)
        {
            var keys = _server.Keys(pattern: "*" + prefix + "*");
            return keys.Select(x => GetValue(x)).ToList();
        }
    }
}
