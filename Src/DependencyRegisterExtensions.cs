using FS.DI.Registration;
using System;
using System.Reflection;

namespace FS.DI.Core
{
    /// <summary>
    ///注册器扩展 
    /// </summary>
    public static class DependencyRegisterExtensions
    {
        /// <summary>
        ///     注册类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType(this IDependencyRegister dependencyRegister, Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            return dependencyRegister.RegisterType(serviceType, serviceType);
        }

        /// <summary>
        ///      注册类型和实现类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType(this IDependencyRegister dependencyRegister, Type serviceType, Type implementationType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            var registerConfiguration = DependencyRegistrationFactory.ForType(serviceType, implementationType);
            return RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///      注册类型和实现类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType<TService, TImplementation>(this IDependencyRegister dependencyRegister)
            where TImplementation : TService
        {
            return dependencyRegister.RegisterType(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        ///     注册类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType<TService>(this IDependencyRegister dependencyRegister)
        {
            return dependencyRegister.RegisterType(typeof(TService));
        }

        /// <summary>
        ///     注册实例为依赖服务
        /// </summary>
        public static ISingletonRegistration<ISingletonRegistration> RegisterInstance(this IDependencyRegister dependencyRegister, Object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            return dependencyRegister.RegisterInstance(instance.GetType(), instance);
        }

        /// <summary>
        ///     注册类型和实现实例为依赖服务
        /// </summary>
        public static ISingletonRegistration<ISingletonRegistration> RegisterInstance(this IDependencyRegister dependencyRegister, Type serviceType, Object implementationInstance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationInstance == null) throw new ArgumentNullException(nameof(implementationInstance));

            var registerConfiguration = DependencyRegistrationFactory.ForInstance(serviceType, implementationInstance);
            return (DependencyRegistration)RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///     注册类型和实现实例为依赖服务
        /// </summary>
        public static ISingletonRegistration<ISingletonRegistration> RegisterInstance<TService>(this IDependencyRegister dependencyRegister, Object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            return dependencyRegister.RegisterInstance(typeof(TService), instance);
        }

        /// <summary>
        ///     注册类型和返回实现实例的委托为依赖服务
        /// </summary>
        public static ILifetimeRegistration<ILifetimeRegistration> RegisterDelegate<TImplementation>(this IDependencyRegister dependencyRegister, Type serviceType, Func<IDependencyResolver, TImplementation> implementationDelegate)
            where TImplementation : class
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationDelegate == null) throw new ArgumentNullException(nameof(implementationDelegate));

            var registerConfiguration = DependencyRegistrationFactory.ForDelegate(serviceType, implementationDelegate);
            return (DependencyRegistration)RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///     注册类型和返回实现实例的委托为依赖服务
        /// </summary>
        public static ILifetimeRegistration<ILifetimeRegistration> RegisterDelegate<TService, TImplementation>(this IDependencyRegister dependencyRegister, Func<IDependencyResolver, TImplementation> implementationDelegate)
            where TImplementation : class, TService
        {
            if (implementationDelegate == null) throw new ArgumentNullException(nameof(implementationDelegate));

            var registerConfiguration = DependencyRegistrationFactory.ForDelegate(typeof(TService), implementationDelegate);
            return (DependencyRegistration)RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///     注册程序集中类型作为依赖服务
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assembly);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///     注册程序集中使用特定命名约定的类型作为依赖服务
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Assembly assembly, String name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assembly, name);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///    注册程序集中使实现特定接口或基类的类型作为依赖服务
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Assembly assembly, Type baseType)
        {
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assembly, baseType);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///        注册程序集中符合过滤条件的类型作为依赖服务    
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Assembly assembly, Func<Type, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assembly, predicate, null);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///    注册程序集中使实现特定接口或基类的类型作为依赖服务
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly<TBaseService>(this IDependencyRegister dependencyRegister, Assembly assembly)
          where TBaseService : class
        {
            return dependencyRegister.RegisterAssembly(assembly, typeof(TBaseService));
        }

        private static IDependencyRegistration RegisterRegistration(IDependencyRegister dependencyRegister, IDependencyRegistration registerConfiguration)
        {
            if (dependencyRegister == null) throw new ArgumentNullException(nameof(IDependencyRegister));
            if (registerConfiguration == null) throw new ArgumentNullException(nameof(registerConfiguration));

            dependencyRegister.RegisterEntry(((DependencyRegistration)registerConfiguration).Entry);
            return registerConfiguration;
        }

        private static IEnumerableRegistration RegisterCollection(IDependencyRegister dependencyRegister, IEnumerableRegistration configurationCollections)
        {
            if (dependencyRegister == null) throw new ArgumentNullException(nameof(IDependencyRegister));
            if (configurationCollections == null) throw new ArgumentNullException(nameof(configurationCollections));

            foreach (var configuration in configurationCollections)
            {
                RegisterRegistration(dependencyRegister, configuration);
            }

            return configurationCollections;
        }
    }
}
