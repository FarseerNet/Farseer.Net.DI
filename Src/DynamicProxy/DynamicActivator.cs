using FS.Cache;
using System;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     包含特定的方法，用以创建对象的代理类型实例。
    /// </summary>
    public sealed class DynamicActivator
    {
        /// <summary>
        ///     使用指定类型的默认构造函数来创建该类型的代理实例。
        /// </summary>
        public static object CreateProxyInstance(Type type)
        {
            return CreateProxyInstance(type, null, null);
        }

        /// <summary>
        ///     使用与指定参数匹配程度最高的构造函数创建指定类型的代理实例。
        /// </summary>
        public static object CreateProxyInstance(Type type, object[] args)
        {
            return CreateProxyInstance(type, args, null);
        }

        /// <summary>
        ///     使用指定类型的默认构造函数和自定义拦截器来创建该类型的代理实例。
        /// </summary>
        public static object CreateProxyInstance(Type type, ICustomInterceptor[] interecptors)
        {
            return CreateProxyInstance(type, null, interecptors);
        }

        /// <summary>
        ///     使用与指定参数匹配程度最高的构造函数和自定义拦截器创建指定类型的代理实例。
        /// </summary>
        public static object CreateProxyInstance(Type type, object[] args, ICustomInterceptor[] interecptors)
        {
            CustomInterceptorCacheManager.Cache(type, interecptors);
            var proxyType = DynamicTypeCacheManager.Cache(type);
            return InstanceCacheManger.Cache(proxyType, args);
        }
    }
}
