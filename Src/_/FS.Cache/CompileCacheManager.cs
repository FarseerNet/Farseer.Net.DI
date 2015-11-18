using FS.DI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Cache
{
    internal sealed class CompileCacheManager : AbsCacheManger<DependencyEntry, Func<IDependencyResolver, Object[], Object>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();
        private CompileCacheManager(DependencyEntry key)
            :base(key)
        {
        }
        protected override Func<IDependencyResolver, object[], object> SetCacheLock()
        {
           lock(_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];
                return default(Func<IDependencyResolver, object[], object>);
            }
        }

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
        public static Object GetCache(DependencyEntry key, IDependencyResolver resolver, Object[] args)
        {
            var factory = new CompileCacheManager(key).GetValue();
            if (factory == null) return null;
            return factory.Invoke(resolver, args);
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static Func<IDependencyResolver, object[], object> GetOrSetCache(DependencyEntry key, Func<Func<IDependencyResolver, Object[], Object>> valueFactory)
        {
            var factory = new CompileCacheManager(key).GetValue();
            if (factory == null)
            {
                lock (_sync)
                {
                    CompileCacheManager.Update(key, (factory = valueFactory()));
                }
            }
            return factory;
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(DependencyEntry key)
        {
            new CompileCacheManager(key).RemoveLock();
        }
    }
}
