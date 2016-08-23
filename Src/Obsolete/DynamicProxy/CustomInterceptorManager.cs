using FS.Cache;
using System;

namespace FS.DI.DynamicProxy
{
    public static class CustomInterceptorManager
    {
        /// <summary>
        ///     配置自定义拦截器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Configure(Type type, params ICustomInterceptor[] interceptors)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (interceptors == null || interceptors.Length == 0) throw new ArgumentNullException(nameof(interceptors));
            CustomInterceptorCacheManager.SetCache(type, interceptors);
        }

        /// <summary>
        ///     卸载配置
        /// </summary>
        /// <param name="type"></param>
        public static void UnConfigure(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            CustomInterceptorCacheManager.RemoveCache(type);
        }
    }
}
