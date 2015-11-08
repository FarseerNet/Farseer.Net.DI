using FS.Cache;
using FS.DI.DynamicProxy;
using FS.DI.Registration;
using System;
using System.Linq;

namespace FS.DI.Core
{
    public static class DynamicProxyRegistrationExtensions
    {
        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        /// <typeparam name="TInterceptor">自定义拦截器</typeparam>
        /// <param name="interceptors">自定义拦截器</param>
        public static IDependencyRegistration AsDynamicProxy<TInterceptor>(this IDependencyRegistration registration, TInterceptor[] interceptors)
            where TInterceptor : ICustomInterceptor
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (interceptors == null) throw new ArgumentNullException(nameof(interceptors));
            registration.AsDynamicProxy();
            CustomInterceptorCacheManager.Cache((registration as DependencyRegistration).
                Entry.ServiceType,
                interceptors.Select<TInterceptor, ICustomInterceptor>(i => i).ToArray());
            return registration;
        }

        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        /// <typeparam name="TInterceptor">自定义拦截器</typeparam>
        /// <param name="interceptors">自定义拦截器</param>
        public static IEnumerableRegistration AsDynamicProxy<TInterceptor>(this IEnumerableRegistration registration, TInterceptor[] interceptors)
             where TInterceptor : ICustomInterceptor
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (interceptors == null) throw new ArgumentNullException(nameof(interceptors));
            foreach(var register in registration)
            {
                register.AsDynamicProxy<TInterceptor>(interceptors);
            }
            return registration;
        }

    }
}
