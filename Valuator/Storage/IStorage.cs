using System.Collections.Generic;

namespace Valuator.Storage
{
    public interface IStorage
    {
        void Load(string key, string value);
        string GetValue(string key);
        List<string> GetValues(string prefix);
    }
}
