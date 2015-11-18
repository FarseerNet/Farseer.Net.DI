using FS.DI.Core;
using System;

namespace FS.Cache
{
    /// <summary>
    ///     作用域缓存管理
    /// </summary>
    internal sealed class ScopedCacheManager : AbsCacheManger<Tuple<IScopedResolver, DependencyEntry>, Object>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();
        private ScopedCacheManager(Tuple<IScopedResolver, DependencyEntry> key)
            : base(key)
        {
        }
        protected override object SetCacheLock()
        {
            lock(_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return null;
            }
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        private void RemoveLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key))
                {
                    if (Key.Item2.ImplementationInstance == null)
                    {
                        var disposable = CacheList[Key] as IDisposable;
                        if (disposable != null)
                            disposable.Dispose();
                    }
                    CacheList.Remove(Key);
                }
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static Object GetCache(IScopedResolver resolver, DependencyEntry entry)
        {
            return new ScopedCacheManager(new Tuple<IScopedResolver, DependencyEntry>(resolver, entry)).GetValue();
        }

        /// <summary>
        ///     添加缓存
        /// </summary>
        public static void SetCache(IScopedResolver resolver, DependencyEntry entry, Object value)
        {
            var key = new Tuple<IScopedResolver, DependencyEntry>(resolver, entry);
            lock(_sync)
            {
                ScopedCacheManager.Update(key, value);
            }          
            ScopedKeyCacheManager.SetCache(resolver, key);
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(Tuple<IScopedResolver, DependencyEntry> key)
        {
            new ScopedCacheManager(key).RemoveLock();
        }
    }
}
