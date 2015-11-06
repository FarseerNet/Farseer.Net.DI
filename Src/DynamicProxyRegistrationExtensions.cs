using FS.Cache;
using FS.DI.DynamicProxy;
using FS.DI.Register;
using System;
using System.Linq;

namespace FS.DI.Core
{
    public static class DynamicProxyRegistrationExtensions
    {
        /// <summary>
        ///      注册为可被动态代理的类型
        /// </summary>
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
        ///      注册为可被动态代理的类型
        /// </summary>
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
