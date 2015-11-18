using FS.DI.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.Cache
{
    /// <summary>
    ///     自定义拦截器缓存管理
    /// </summary>
    internal class CustomInterceptorCacheManager : AbsCacheManger<Type, IEnumerable<ICustomInterceptor>>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();

        private CustomInterceptorCacheManager(Type key)
            : base(key)
        {
        }

        protected override IEnumerable<ICustomInterceptor> SetCacheLock()
        {
            lock (_sync)
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
                if (CacheList.ContainsKey(Key)) CacheList.Remove(Key);
                else throw new ArgumentException("尝移除不存在的配置失败。");
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static IEnumerable<ICustomInterceptor> GetCache(Type key)
        {
            var cache = new CustomInterceptorCacheManager(key).GetValue();
            return cache == null ? new ICustomInterceptor[0] : cache;
        }

        /// <summary>
        ///     设置缓存
        /// </summary>
        public static void SetCache(Type key, ICustomInterceptor[] interceptors)
        {
            if (interceptors == null || interceptors.Length == 0)
                throw new ArgumentNullException(nameof(interceptors));
            var cache = new CustomInterceptorCacheManager(key).GetValue();
            if (cache == null)
            {
                var set = new HashSet<ICustomInterceptor>();
                foreach (var item in interceptors)
                    set.Add(item);
                Update(key, set);
            }
            else
            {
                foreach (var item in interceptors)
                    (cache as HashSet<ICustomInterceptor>).Add(item);
            }
        }

        public static void RemoveCache(Type key)
        {
            new CustomInterceptorCacheManager(key).RemoveLock();
        }
    }
}
