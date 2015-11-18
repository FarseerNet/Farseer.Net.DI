using FS.DI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.Extends;

namespace FS.Cache
{
    internal sealed class PropertyCacheManager : AbsCacheManger<DependencyEntry, IEnumerable<PropertyInfo>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();
        private PropertyCacheManager(DependencyEntry key)
            :base(key)
        {
        }
        protected override IEnumerable<PropertyInfo> SetCacheLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return (CacheList[Key] = new List<PropertyInfo>(0));
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
        ///     获取缓存
        /// </summary>
        public static IEnumerable<PropertyInfo> GetOrSetCache(DependencyEntry key, Func<IEnumerable<PropertyInfo>> factory)
        {
            var cache = new PropertyCacheManager(key).GetValue();
            if (cache.Any())
            {
                return cache;
            }
            lock (_sync)
            {
                var list = cache as List<PropertyInfo>;
                factory().ForEach(item => list.Add(item));
                return cache;
            }
        }

        /// <summary>
        ///     缓存是否包含任何元素
        /// </summary>
        public static bool Any(DependencyEntry key)
        {
            return new PropertyCacheManager(key).GetValue().Any();
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(DependencyEntry entry)
        {
            entry.Chain().ForEach(key => new PropertyCacheManager(key).RemoveLock());
        }
    }
}
