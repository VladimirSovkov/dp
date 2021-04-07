using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valuator.Toolkit.Storage
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");

        private readonly string _hostRu = Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User);
        private readonly string _hostEu = Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User);
        private readonly string _hostOther = Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User);

        private readonly IDatabase _db;
        private readonly Dictionary<string, IDatabase> _dateBases;

        public RedisStorage()
        {
            _db = _connection.GetDatabase();
            _dateBases = GetConnections();
        }

        public void AddValueByKey(string id, string key, string value)
        {
            string shardKey = GetShardKeyById(id);
            var dateBase = _dateBases[shardKey];
            dateBase.StringSet(key, value);
        }

        public void AddShardKeyById(string id, string shardKey)
        {
            _db.StringSet(id, shardKey);
        }

        public string GetShardKeyById(string id)
        {
            return _db.StringGet(id);
        }

        public string GetValue(string id, string key)
        {
            return _dateBases[GetShardKeyById(id)].StringGet(key);
        }

        public void AddTextToSet(string id, string text)
        {
            _dateBases[GetShardKeyById(id)].SetAdd(Constants.KEY_OF_ALL_TEXT, text);
        }

        public bool IsContainsText(string text)
        {
            foreach (var db in _dateBases.Values)
            {
                if (db.SetContains(Constants.KEY_OF_ALL_TEXT, text))
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<string, IDatabase> GetConnections()
        {
            return new Dictionary<string, IDatabase>
            {
                [Constants.RU_SEGMENT] = ConnectionMultiplexer.Connect(_hostRu).GetDatabase(),
                [Constants.EU_SEGMENT] = ConnectionMultiplexer.Connect(_hostEu).GetDatabase(),
                [Constants.OTHER_SEGMENT] = ConnectionMultiplexer.Connect(_hostOther).GetDatabase()
            };
        }
    }
}
