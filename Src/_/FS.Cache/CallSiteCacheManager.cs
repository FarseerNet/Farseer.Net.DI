using FS.DI.Core;
using FS.DI.Resolve;
using FS.Extends;
using System;
using System.Collections.Generic;

namespace FS.Cache
{
    /// <summary>
    ///     解析器调用缓存管理
    /// </summary>
    internal sealed class CallSiteCacheManager : AbsCacheManger<IDependencyResolver, IEnumerable<IResolverCallSite>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();
        private CallSiteCacheManager(IDependencyResolver key)
            : base(key)
        {
        }

        /// <summary>
        ///     获取缓存   
        /// </summary>
        protected override IEnumerable<IResolverCallSite> SetCacheLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];

                return (CacheList[Key] = new List<IResolverCallSite>());
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
        public static void SetCache(IDependencyResolver key, params IResolverCallSite[] callSites)
        {         
            if (callSites == null || callSites.Length == 0)
                return;
            lock (_sync)
            {
                var collection = new CallSiteCacheManager(key).GetValue();
                callSites.ForEach(callSite => (collection as ICollection<IResolverCallSite>).Add(callSite));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static IEnumerable<IResolverCallSite> GetCache(IDependencyResolver key)
        {
           return new CallSiteCacheManager(key).GetValue();          
        }

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IDependencyResolver key)
        {
            new CallSiteCacheManager(key).RemoveLock();
        }
    }
}
