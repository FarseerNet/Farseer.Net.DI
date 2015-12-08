using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.Extends;
using FS.DI;

namespace FS.Cache
{
    internal sealed class PropertyCacheManager : AbsCacheManger<Dependency, IEnumerable<PropertyInfo>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object Sync = new object();

        private PropertyCacheManager(Dependency key)
            : base(key)
        {
        }

        protected override IEnumerable<PropertyInfo> SetCacheLock()
        {
            lock (Sync)
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
            lock (Sync)
            {
                if (CacheList.ContainsKey(Key)) CacheList.Remove(Key);
            }
        }



        /// <summary>
        ///     获取缓存
        /// </summary>
        public static IEnumerable<PropertyInfo> GetOrSetCache(Dependency key,
            Func<IEnumerable<PropertyInfo>> factory)
        {
            var cache = new PropertyCacheManager(key).GetValue();
            var propertyInfos = cache as IList<PropertyInfo> ?? cache.ToList();
            if (propertyInfos.Any())
            {
                return propertyInfos;
            }
            lock (Sync)
            {
                var list = cache as List<PropertyInfo>;
                factory().ForEach(item => list?.Add(item));
                return propertyInfos;
            }
        }

        /// <summary>
        ///     缓存是否包含任何元素
        /// </summary>
        public static bool Any(Dependency key) => new PropertyCacheManager(key).GetValue().Any();

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(Dependency dependency)
            => dependency.Chain().ForEach(key => new PropertyCacheManager(key).RemoveLock());
    }
}
