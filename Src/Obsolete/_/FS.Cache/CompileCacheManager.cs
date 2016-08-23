using FS.DI;
using System;

namespace FS.Cache
{
    internal sealed class CompileCacheManager :
        AbsCacheManger<Dependency, Func<IDependencyResolver, object[], object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object Sync = new object();

        private CompileCacheManager(Dependency key)
            : base(key)
        {
        }

        protected override Func<IDependencyResolver, object[], object> SetCacheLock()
        {
            lock (Sync)
            {
                return CacheList.ContainsKey(Key)
                    ? CacheList[Key]
                    : default(Func<IDependencyResolver, object[], object>);
            }
        }

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
        public static object GetCache(Dependency key, IDependencyResolver resolver, object[] args)
        {
            var factory = new CompileCacheManager(key).GetValue();
            return factory?.Invoke(resolver, args);
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static Func<IDependencyResolver, object[], object> GetOrSetCache(Dependency key,
            Func<Func<IDependencyResolver, object[], object>> valueFactory)
        {
            var factory = new CompileCacheManager(key).GetValue();
            if (factory != null) return factory;
            lock (Sync)
            {
                Update(key, (factory = valueFactory()));
            }
            return factory;
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(Dependency key) => new CompileCacheManager(key).RemoveLock();
    }
}
