using System;
using System.Collections.Generic;

namespace LRUGenericCache
{
    class Program
    {
        static async void TestAsync()
        {
            LRUSingletonMemoryCache cache = LRUSingletonMemoryCache.Instance;
            List<Tuple<string, object>> data = new List<Tuple<string, object>> { Tuple.Create("1", (object)1),
            Tuple.Create("2", (object)2),
            Tuple.Create("3", (object)3),
            Tuple.Create("4", (object)4)};
            cache.PopulateAsync(data);
            Console.WriteLine("AddOrUpdate 2");
            await cache.AddOrUpdate("2", 20);
            Console.WriteLine("AddOrUpdate 5");
            await cache.AddOrUpdate("5", "x50");
            Console.WriteLine("AddOrUpdate 6");
            await cache.AddOrUpdate("6", "6");
            Console.WriteLine("AddOrUpdate 3");
            await cache.AddOrUpdate("3", "x30");
            Console.WriteLine("AddOrUpdate 4");
            await cache.AddOrUpdate("4", 40);
            Console.WriteLine("AddOrUpdate 5");
            await cache.AddOrUpdate("5", new List<int> { 500, 50, 5 });
            Console.WriteLine("AddOrUpdate 3");
            await cache.AddOrUpdate("3", 300);
            object result;
            string key = "5";
            Console.WriteLine("TryGet 5");
            bool status = cache.TryGet(key, out result);
            Console.WriteLine($"Get Value: {status}, {key}:{result}");
            Console.WriteLine($"Cache Size: {LRUSingletonMemoryCache.Count}");
            Console.WriteLine($"Cache Cleared: {cache.ClearAsync()}, Cache Size: {LRUSingletonMemoryCache.Count}");
        }

        static void Main(string[] args)
        {
            TestAsync();
        }
    }
}
