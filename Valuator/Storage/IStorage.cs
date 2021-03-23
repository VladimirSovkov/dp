using System.Collections.Generic;

namespace Valuator.Storage
{
    public interface IStorage
    {
        void AddByKey(string key, string value);
        string GetValue(string key);
        List<string> GetValues(string prefix);
    }
}
