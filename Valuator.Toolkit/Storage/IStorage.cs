using System.Collections.Generic;

namespace Valuator.Toolkit.Storage
{
    public interface IStorage
    {
        void AddValueByKey(string shardKey, string key, string value);
        void AddShardKeyById(string key, string shardId);
        string GetShardKeyById(string key);
        string GetValue(string shardKey, string key);
        void AddTextToSet(string shardKey, string text);
        bool IsContainsText(string text);
    }
}
