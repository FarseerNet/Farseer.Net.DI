
using FS.DI;
using FS.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.Cache
{
    /// <summary>
    ///     作用域Key缓存管理
    /// </summary>
    internal sealed class ScopedKeyCacheManager :
        AbsCacheManger<IScopedResolver, IEnumerable<Tuple<IScopedResolver, Dependency>>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object Sync = new object();

        private ScopedKeyCacheManager(IScopedResolver key)
            : base(key)
        {
        }

        protected override IEnumerable<Tuple<IScopedResolver, Dependency>> SetCacheLock()
        {
            lock (Sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return CacheList[Key] = new List<Tuple<IScopedResolver, Dependency>>();
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
        public static void SetCache(IScopedResolver key, Tuple<IScopedResolver, Dependency> scopedKey = null)
        {
            var collection = new ScopedKeyCacheManager(key).GetValue();
            if (scopedKey == null) return;
            lock (Sync)
            {
                ((List<Tuple<IScopedResolver, Dependency>>) collection).Add(scopedKey);
            }
        }

        /// <summary>
        ///     读取缓存
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static IEnumerable<Tuple<IScopedResolver, Dependency>> GetCache(IScopedResolver key)
        {
            return new ScopedKeyCacheManager(key).GetValue();
        }


        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IScopedResolver key)
        {
            var keys = GetCache(key);
            keys.ForEach(ScopedCacheManager.RemoveCache);
            new ScopedKeyCacheManager(key).RemoveLock();
        }
    }
}
