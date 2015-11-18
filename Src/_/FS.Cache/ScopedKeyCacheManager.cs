using FS.DI.Core;
using FS.Extends;
using System;
using System.Collections.Generic;

namespace FS.Cache
{
    /// <summary>
    ///     作用域Key缓存管理
    /// </summary>
    internal sealed class ScopedKeyCacheManager : AbsCacheManger<IScopedResolver, IEnumerable<Tuple<IScopedResolver, DependencyEntry>>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();

        private ScopedKeyCacheManager(IScopedResolver key)
            : base(key)
        {
        }
        protected override IEnumerable<Tuple<IScopedResolver, DependencyEntry>> SetCacheLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return CacheList[Key] = new List<Tuple<IScopedResolver, DependencyEntry>>();
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
        public static void SetCache(IScopedResolver key, Tuple<IScopedResolver, DependencyEntry> scopedKey = null)
        {
            var collection = new ScopedKeyCacheManager(key).GetValue();
            if (scopedKey != null)
            {
                lock(_sync)
                {
                    (collection as List<Tuple<IScopedResolver, DependencyEntry>>).Add(scopedKey);
                }
            }               
        }

        /// <summary>
        ///     读取缓存
        /// </summary>
        public static IEnumerable<Tuple<IScopedResolver, DependencyEntry>> GetCache(IScopedResolver key)
        {
            return new ScopedKeyCacheManager(key).GetValue();
        }


        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IScopedResolver key)
        {
            var keys = ScopedKeyCacheManager.GetCache(key);
            keys.ForEach(k => ScopedCacheManager.RemoveCache(k));
            new ScopedKeyCacheManager(key).RemoveLock();
        }
    }
}
