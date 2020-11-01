using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LRUGenericCache
{
    class Program
    {
        static void SimulateCaching()
        {
            Random rnd = new Random();
            int sleepTime = 10000;
            LRUSingletonMemoryCache cache = LRUSingletonMemoryCache.Instance;
            List<Tuple<string, object>> data = new List<Tuple<string, object>> { Tuple.Create("1", (object)1),
            Tuple.Create("2", (object)2),
            Tuple.Create("3", (object)3),
            Tuple.Create("4", (object)4)};
            cache.Populate(data);
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 2");
            cache.AddOrUpdate("2", 20);
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 5");
            cache.AddOrUpdate("5", "x50");
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 6");
            cache.AddOrUpdate("6", "6");
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 3");
            cache.AddOrUpdate("3", "x30");
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 4");
            cache.AddOrUpdate("4", 40);
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 5");
            cache.AddOrUpdate("5", new List<int> { 500, 50, 5 });
            Thread.Sleep(rnd.Next(sleepTime));
            Console.WriteLine("AddOrUpdate 3");
            cache.AddOrUpdate("3", 300);
            object result;
            string key = "5";
            Console.WriteLine("TryGet 5");
            bool status = cache.TryGet(key, out result);
            Console.WriteLine($"Get Value: {status}, {key}:{result}");
            Console.WriteLine($"Cache Size: {LRUSingletonMemoryCache.Count}");
            Console.WriteLine($"Cache Cleared: {cache.Clear()}, Cache Size: {LRUSingletonMemoryCache.Count}");
        }

        static void TestCaching()
        {
            var task1 = Task.Factory.StartNew(() => SimulateCaching());
            var task2 = Task.Factory.StartNew(() => SimulateCaching());
            Task.WaitAll(task1, task2);
        }

        static void Main(string[] args)
        { 
            TestCaching();
        }
    }
}
