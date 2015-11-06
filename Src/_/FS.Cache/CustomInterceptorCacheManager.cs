using FS.DI.DynamicProxy;
using System;
using System.Collections.Generic;

namespace FS.Cache
{
    internal class CustomInterceptorCacheManager : AbsCacheManger<Type, IEnumerable<ICustomInterceptor>>
    {
        private static readonly Object _sync = new Object();

        public CustomInterceptorCacheManager(Type key)
            : base(key)
        {
        }
        protected override IEnumerable<ICustomInterceptor> SetCacheLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];

                return (CacheList[Key] = new HashSet<ICustomInterceptor>());
            }
        }

        public static IEnumerable<ICustomInterceptor> Cache(Type key, params ICustomInterceptor[] interceptors)
        {
            var set = new CustomInterceptorCacheManager(key).GetValue();
            if (interceptors == null || interceptors.Length == 0) return set;
            lock(_sync)
            {
                foreach (var item in interceptors) ((HashSet<ICustomInterceptor>)set).Add(item);
                return set;
            }         
        }
    }
}
