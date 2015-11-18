using FS.Cache;
using System;

namespace FS.DI.DynamicProxy
{
    public sealed class CustomInterceptorManager
    {
        public static void Configure(Type type, params ICustomInterceptor[] interceptors)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (interceptors == null || interceptors.Length == 0) throw new ArgumentNullException(nameof(interceptors));
            CustomInterceptorCacheManager.SetCache(type, interceptors);
        }

        public static void UnConfigure(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            CustomInterceptorCacheManager.RemoveCache(type);
        }
    }
}
