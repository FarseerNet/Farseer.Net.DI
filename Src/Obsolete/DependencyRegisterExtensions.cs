using FS.DI.Registration;
using System;
using System.Reflection;

namespace FS.DI
{
    /// <summary>
    ///注册器扩展 
    /// </summary>
    public static class DependencyRegisterExtensions
    {
        /// <summary>
        ///     注册类型为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDependencyRegistration RegisterType(this IDependencyRegister dependencyRegister, Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            return dependencyRegister.RegisterType(serviceType, serviceType);
        }

        /// <summary>
        ///      注册类型和实现类型为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDependencyRegistration RegisterType(this IDependencyRegister dependencyRegister, Type serviceType,
            Type implementationType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            var registerConfiguration = DependencyRegistrationFactory.ForType(serviceType, implementationType);
            return RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///      注册类型和实现类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType<TService, TImplementation>(
            this IDependencyRegister dependencyRegister)
            where TImplementation : TService
            => dependencyRegister.RegisterType(typeof(TService), typeof(TImplementation));

        /// <summary>
        ///     注册类型为依赖服务
        /// </summary>
        public static IDependencyRegistration RegisterType<TService>(this IDependencyRegister dependencyRegister)
            => dependencyRegister.RegisterType(typeof(TService));

        /// <summary>
        ///     注册实例为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static ISingletonRegistration<ISingletonRegistration> RegisterInstance(
            this IDependencyRegister dependencyRegister, object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            return dependencyRegister.RegisterInstance(instance.GetType(), instance);
        }

        /// <summary>
        ///     注册类型和实现实例为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static ISingletonRegistration<ISingletonRegistration> RegisterInstance(
            this IDependencyRegister dependencyRegister, Type serviceType, object implementationInstance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationInstance == null) throw new ArgumentNullException(nameof(implementationInstance));

            var registerConfiguration = DependencyRegistrationFactory.ForInstance(serviceType, implementationInstance);
            return (DependencyRegistration)RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///     注册类型和返回实现实例的委托为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static ILifetimeRegistration<ILifetimeRegistration> RegisterDelegate<TImplementation>(
            this IDependencyRegister dependencyRegister, Type serviceType,
            Func<IDependencyResolver, TImplementation> implementationDelegate)
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
        /// <exception cref="ArgumentNullException"></exception>
        public static ILifetimeRegistration<ILifetimeRegistration> RegisterDelegate<TService, TImplementation>(
            this IDependencyRegister dependencyRegister,
            Func<IDependencyResolver, TImplementation> implementationDelegate)
            where TImplementation : class, TService
        {
            if (implementationDelegate == null) throw new ArgumentNullException(nameof(implementationDelegate));

            var registerConfiguration = DependencyRegistrationFactory.ForDelegate(typeof(TService),
                implementationDelegate);
            return (DependencyRegistration)RegisterRegistration(dependencyRegister, registerConfiguration);
        }

        /// <summary>
        ///     注册程序集中类型作为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister,
          params Assembly[] assemblys)
        {
            if (assemblys == null) throw new ArgumentNullException(nameof(assemblys));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assemblys);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///     注册程序集中使用特定命名约定的类型作为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, string name, params Assembly[] assemblys)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (assemblys == null) throw new ArgumentNullException(nameof(assemblys));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assemblys, name);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///    注册程序集中使实现特定接口或基类的类型作为依赖服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Type baseType, params Assembly[] assemblys)
        {
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));
            if (assemblys == null) throw new ArgumentNullException(nameof(assemblys));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assemblys, baseType);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///        注册程序集中符合过滤条件的类型作为依赖服务    
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerableRegistration RegisterAssembly(this IDependencyRegister dependencyRegister, Func<Type, bool> predicate,
            params Assembly[] assemblys)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (assemblys == null) throw new ArgumentNullException(nameof(assemblys));

            var enumerableConfiguration = DependencyRegistrationFactory.ForAssembly(assemblys, predicate, null);
            return RegisterCollection(dependencyRegister, enumerableConfiguration);
        }

        /// <summary>
        ///    注册程序集中使实现特定接口或基类的类型作为依赖服务
        /// </summary>
        public static IEnumerableRegistration RegisterAssembly<TBaseService>(
            this IDependencyRegister dependencyRegister, params Assembly[] assemblys)
            where TBaseService : class => dependencyRegister.RegisterAssembly(typeof(TBaseService), assemblys);

        private static IDependencyRegistration RegisterRegistration(IDependencyRegister dependencyRegister,
            IDependencyRegistration registerConfiguration)
        {
            if (dependencyRegister == null) throw new ArgumentNullException(nameof(IDependencyRegister));
            if (registerConfiguration == null) throw new ArgumentNullException(nameof(registerConfiguration));

            dependencyRegister.RegisterDependency((registerConfiguration as DependencyRegistration)?.Dependency);
            return registerConfiguration;
        }

        private static IEnumerableRegistration RegisterCollection(IDependencyRegister dependencyRegister,
            IEnumerableRegistration configurationCollections)
        {
            if (dependencyRegister == null) throw new ArgumentNullException(nameof(IDependencyRegister));
            if (configurationCollections == null) throw new ArgumentNullException(nameof(configurationCollections));

            foreach (var configuration in configurationCollections)
                RegisterRegistration(dependencyRegister, configuration);

            return configurationCollections;
        }
    }
}
