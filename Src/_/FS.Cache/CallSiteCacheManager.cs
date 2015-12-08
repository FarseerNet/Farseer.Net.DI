using FS.DI;
using FS.DI.Resolve;
using FS.Extends;
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
        private static readonly object Sync = new object();

        private CallSiteCacheManager(IDependencyResolver key)
            : base(key)
        {
        }

        /// <summary>
        ///     获取缓存   
        /// </summary>
        protected override IEnumerable<IResolverCallSite> SetCacheLock()
        {
            lock (Sync)
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
            lock (Sync)
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
            lock (Sync)
            {
                var collection = new CallSiteCacheManager(key).GetValue();
                callSites.ForEach(callSite => ((ICollection<IResolverCallSite>) collection).Add(callSite));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static IEnumerable<IResolverCallSite> GetCache(IDependencyResolver key)
            => new CallSiteCacheManager(key).GetValue();

        /// <summary>
        ///     删除缓存
        /// </summary>
        public static void RemoveCache(IDependencyResolver key) => new CallSiteCacheManager(key).RemoveLock();
    }
}
