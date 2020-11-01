using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LRUGenericCache
{
    public interface ILRUCache
    {
        bool TryGet(string key, out object result);

        void AddOrUpdate(string key, object value);

        static long Count { get; }

        bool ContainsKey(string key);

        bool Clear();

        bool Populate(List<Tuple<string, object>> data);
    }
}
