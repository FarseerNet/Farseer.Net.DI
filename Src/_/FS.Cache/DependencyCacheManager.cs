using FS.DI;
using FS.Extends;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FS.Cache
{
    /// <summary>
    ///     依赖服务对象缓存管理
    /// </summary>
    internal sealed class DependencyCacheManager :
        AbsCacheManger<IScopedResolver, IDictionary<Type, Dependency>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object Sync = new object();

        private DependencyCacheManager(IScopedResolver key)
            : base(key)
        {
        }

        protected override IDictionary<Type, Dependency> SetCacheLock()
        {
            lock (Sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return (CacheList[Key] = new ConcurrentDictionary<Type, Dependency>());
            }
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        private void RemoveLock()
        {
            lock (Sync)
            {
                if (CacheList.ContainsKey(Key)) CacheList.Remove(Key);
            }
        }


        /// <summary>
        ///     设置缓存
        /// </summary>
        public static void SetCache(IScopedResolver key, IEnumerable<Dependency> dependencyEntries)
        {
            var cache = new DependencyCacheManager(key).GetValue();
            dependencyEntries.ForEach(dependency => cache.Add(dependency.ServiceType, dependency));
        }

        /// <summary>
        ///    获取缓存
        /// </summary>
        public static Dependency GetCache(IScopedResolver key, Type serviceType)
        {
            var dependencyEntries = new DependencyCacheManager(key).GetValue();
            return dependencyEntries.ContainsKey(serviceType)
                ? dependencyEntries[serviceType]
                : default(Dependency);
        }

        /// <summary>
        ///    获取缓存
        /// </summary>
        public static IEnumerable<Dependency> GetCache(IScopedResolver key)
            => new DependencyCacheManager(key).GetValue().Values;

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IScopedResolver key)
        {
            GetCache(key).ForEach(PropertyCacheManager.RemoveCache);
            new DependencyCacheManager(key).RemoveLock();
        }
    }
}
