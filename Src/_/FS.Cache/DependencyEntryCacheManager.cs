using FS.DI.Core;
using FS.Extends;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FS.Cache
{
    /// <summary>
    ///     依赖服务对象缓存管理
    /// </summary>
    internal sealed class DependencyEntryCacheManager : AbsCacheManger<IScopedResolver, IDictionary<Type, DependencyEntry>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();

        private DependencyEntryCacheManager(IScopedResolver key)
            :base(key)
        {
        }
        protected override IDictionary<Type, DependencyEntry> SetCacheLock()
        {
            lock(_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return (CacheList[Key] = new ConcurrentDictionary<Type, DependencyEntry>());
            }
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        private void RemoveLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key)) CacheList.Remove(Key);
            }
        }


        /// <summary>
        ///     设置缓存
        /// </summary>
        public static void SetCache(IScopedResolver key, IEnumerable<DependencyEntry> dependencyEntries)
        {
            var cache = new DependencyEntryCacheManager(key).GetValue();
            dependencyEntries.ForEach(entry => cache.Add(entry.ServiceType, entry));
        }

        /// <summary>
        ///    获取缓存
        /// </summary>
        public static DependencyEntry GetCache(IScopedResolver key, Type serviceType)
        {
            var dependencyEntries = new DependencyEntryCacheManager(key).GetValue();
            if (dependencyEntries.ContainsKey(serviceType))
                return dependencyEntries[serviceType];
            return default(DependencyEntry);
        }

        /// <summary>
        ///    获取缓存
        /// </summary>
        public static IEnumerable<DependencyEntry> GetCache(IScopedResolver key)
        {
            return new DependencyEntryCacheManager(key).GetValue().Values;
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IScopedResolver key)
        {
            DependencyEntryCacheManager.GetCache(key).ForEach(entry => PropertyCacheManager.RemoveCache(entry));
            new DependencyEntryCacheManager(key).RemoveLock();
        }
    }
}
