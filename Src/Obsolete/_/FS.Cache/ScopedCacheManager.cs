using FS.DI;
using System;

namespace FS.Cache
{
    /// <summary>
    ///     作用域缓存管理
    /// </summary>
    internal sealed class ScopedCacheManager : AbsCacheManger<Tuple<IScopedResolver, Dependency>, object>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object Sync = new object();

        private ScopedCacheManager(Tuple<IScopedResolver, Dependency> key)
            : base(key)
        {
        }

        protected override object SetCacheLock()
        {
            lock (Sync)
            {
                return CacheList.ContainsKey(Key) ? CacheList[Key] : null;
            }
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        private void RemoveLock()
        {
            lock (Sync)
            {
                if (!CacheList.ContainsKey(Key)) return;
                if (Key.Item2.ImplementationInstance == null)
                {
                    var disposable = CacheList[Key] as IDisposable;
                    disposable?.Dispose();
                }
                CacheList.Remove(Key);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static object GetCache(IScopedResolver resolver, Dependency dependency)
            => new ScopedCacheManager(new Tuple<IScopedResolver, Dependency>(resolver, dependency)).GetValue();

        /// <summary>
        ///     添加缓存
        /// </summary>
        public static void SetCache(IScopedResolver resolver, Dependency dependency, object value)
        {
            var key = new Tuple<IScopedResolver, Dependency>(resolver, dependency);
            lock (Sync)
            {
                Update(key, value);
            }
            ScopedKeyCacheManager.SetCache(resolver, key);
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(Tuple<IScopedResolver, Dependency> key)
            => new ScopedCacheManager(key).RemoveLock();
    }
}
