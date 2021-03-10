using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Valuator.Storage
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
        private readonly IDatabase _db;

        public RedisStorage()
        {
            _db = _connection.GetDatabase();
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
            var server = _connection.GetServer("localhost", 6379);
            var keys = server.Keys(pattern: "*" + prefix + "*");
            return keys.Select(x => GetValue(x)).ToList();
        }
    }
}
