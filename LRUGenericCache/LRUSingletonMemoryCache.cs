﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Caching;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace LRUGenericCache
{
    class LRUSingletonMemoryCache: ILRUCache
    {
        private static readonly ObjectCache _memoryCache;
        private static readonly LRUQueue _lRUQueue;
        private static readonly ConcurrentDictionary<object, SemaphoreSlim> _locks;
        private static readonly CacheItemPolicy _policy;
        private static CacheEntryRemovedCallback removeCallback = null;
        private static long _capacity;

        static LRUSingletonMemoryCache()
        {
            Instance = new LRUSingletonMemoryCache();
            _memoryCache = new MemoryCache("LRUSingletonMemoryCache");
            _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            _policy = new CacheItemPolicy();
            _lRUQueue = new LRUQueue();
            removeCallback = new CacheEntryRemovedCallback(OnRemoved);
            _policy.RemovedCallback = removeCallback;
            _capacity = 4;
        }

        public static long Count
        {
            get { return _memoryCache.GetCount(); }
        }

        public static LRUSingletonMemoryCache Instance { get; private set; }

        public bool TryGet(string key, out object result)
        {
            CacheItemNode cacheItemNode = null;
            if (_memoryCache.Contains(key))
            {
                cacheItemNode = (CacheItemNode)_memoryCache[key];
                _lRUQueue.MoveToFront(cacheItemNode);
                result = cacheItemNode.Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public async void AddOrUpdate(string key, object value)
        {
            SemaphoreSlim item_lock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            await item_lock.WaitAsync();
            try
            {
                var cacheItemNode = new CacheItemNode(key, value);
                if (_memoryCache.Contains(key))
                {
                    CacheItemNode source = (CacheItemNode)_memoryCache.Get(key);
                    _lRUQueue.Remove(source);
                    _lRUQueue.MoveToFront(cacheItemNode);
                    _memoryCache.Remove(key);
                    _memoryCache.Set(key, cacheItemNode, _policy);
                    OnUpdate(source, cacheItemNode);
                }
                else
                {
                    OnAdd(key);
                    _memoryCache.Set(key, cacheItemNode, _policy);
                    _lRUQueue.MoveToFront(cacheItemNode);
                    // For DEBUG purpose
                    Console.WriteLine($"LRU queue state = {_lRUQueue.QueueState()}");
                    Console.WriteLine($"Cache size = {Count}");
                }
            }
            finally
            {
                item_lock.Release();
            }
        }

        public async Task Remove(string key)
        {
            SemaphoreSlim item_lock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            await item_lock.WaitAsync();
            try
            {
                _memoryCache.Remove(key);
            }
            finally
            {
                item_lock.Release();
            }
        }
 
        public bool ContainsKey(string key)
        {
            return _memoryCache.Contains(key);
        }

        public bool Clear()
        {
            try
            {
                var allKeys = _memoryCache.Select(o => o.Key);
                Parallel.ForEach(allKeys, async key => await Remove(key));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Populate(List<Tuple<string, object>> data)
        {
            try
            {
                var key_value_pairs = data.Select(d => (d.Item1, d.Item2));
                Parallel.ForEach(data, d => {
                    Instance.AddOrUpdate(d.Item1, d.Item2);
                });
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static void OnRemoved(CacheEntryRemovedArguments cacheEntryRemovedArguments)
        {
            Console.WriteLine($"Removed Key: {cacheEntryRemovedArguments.CacheItem.Key}");
        }


        public static void OnUpdate(CacheItemNode source, CacheItemNode updated)
        {
            Console.WriteLine($"Key Updated: {source.Key}, Item: (Original: {source.Value}, Updated: {updated.Value})");
        }

 
        public static void OnAdd(string key)
        {
            if (Count >= _capacity)
            {
                CacheItemNode cin = _lRUQueue.RemoveLast();
                _memoryCache.Remove(cin.Key);
                // To ensure that we keep consistency between items in Cache
                // and in _locks dictionary, we remove the entry for the evited 
                // key in _locks.
                if (!cin.Key.Equals(key))
                {
                    SemaphoreSlim semaphore;
                    _locks.TryRemove(key, out semaphore);
                }
            }
        }
     }
}
