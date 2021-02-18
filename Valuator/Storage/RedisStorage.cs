using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Valuator.Storage
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _db;

        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
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
