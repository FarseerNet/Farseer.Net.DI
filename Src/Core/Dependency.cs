using System;
using System.Collections.Generic;

namespace FS.DI
{
    /// <summary>
    ///     依赖服务对象
    /// </summary>
    public sealed class Dependency
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        ///     依赖服务风格
        /// </summary>
        internal DependencyStyle Style { get; set; }

        /// <summary>
        ///     依赖服务实现类型
        /// </summary>
        internal Type ImplementationType { get; }

        /// <summary>
        ///     依赖服务实例
        /// </summary>
        internal object ImplementationInstance { get; }

        /// <summary>
        ///     返回依赖服务实现类委托
        /// </summary>
        internal Func<IDependencyResolver, object> ImplementationDelegate { get; }

        /// <summary>
        ///     依赖服务类型
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///     依赖服务生命周期
        /// </summary>
        public DependencyLifetime Lifetime { get; internal set; }

        /// <summary>
        ///     依赖服务下一个实现
        /// </summary>
        public Dependency Next { get; private set; }

        /// <summary>
        ///     依赖服务实现
        /// </summary>
        public Dependency Last { get; private set; }

        private Dependency(Type serviceType, DependencyLifetime lifetime)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (serviceType.IsGenericTypeDefinition)
            {
                throw new ArgumentOutOfRangeException(nameof(serviceType), "服务类型不能为泛型类型定义。");
            }

            ServiceType = serviceType;
            Lifetime = lifetime;
            Last = this;
        }

        private Dependency(Type serviceType, DependencyLifetime lifetime, Type implementationType)
            : this(serviceType, lifetime)
        {

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (implementationType.IsInterface || implementationType.IsAbstract)
            {
                throw new ArgumentException("服务实现不能为抽象类或接口。", nameof(implementationType));
            }

            if (!serviceType.IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException(string.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName,
                    implementationType.FullName));
            }

            ImplementationType = implementationType;
        }

        private Dependency(Type serviceType, object implementationInstance)
            : this(serviceType, DependencyLifetime.Singleton)
        {
            var implType = implementationInstance.GetType();

            if (!serviceType.IsAssignableFrom(implType))
            {
                throw new InvalidOperationException(string.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName,
                    implType.FullName));
            }

            ImplementationInstance = implementationInstance;
        }

        private Dependency(Type serviceType, DependencyLifetime lifetime,
            Func<IDependencyResolver, object> implementationDelegate)
            : this(serviceType, lifetime)
        {
            ImplementationDelegate = implementationDelegate;

            var implType = GetDelegateReturnType();

            if (!serviceType.IsAssignableFrom(implType))
                throw new InvalidOperationException(string.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName,
                    implType.FullName));
        }

        /// <summary>
        ///     返回依赖服务的实现类型
        /// </summary>
        /// <returns></returns>
        public Type GetImplementationType()
        {
            if (ImplementationType != null)
            {
                return ImplementationType;
            }
            else if (ImplementationInstance != null)
            {
                return ImplementationInstance.GetType();
            }
            else if (ImplementationDelegate != null)
            {
                return GetDelegateReturnType();
            }

            throw new InvalidOperationException($"无法获取{ServiceType.FullName}的实现类型。");
        }

        private Type GetDelegateReturnType()
        {
            var typeArguments = ImplementationDelegate.GetType().GetGenericArguments();

            if (typeArguments.Length == 2)
            {
                return typeArguments[1];
            }

            throw new ArgumentException(nameof(ServiceType));
        }

        /// <summary>
        ///     添加依赖服务
        /// </summary>
        /// <param name="dependency">相同类型的依赖服务</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Add(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            if (ServiceType != dependency.ServiceType)
                throw new ArgumentOutOfRangeException(nameof(dependency), "当前注册的服务类型需于目标服务类型一致。");

            if (GetImplementationType() == dependency.GetImplementationType())
                throw new ArgumentOutOfRangeException(nameof(dependency),
                    message: $"已注册{dependency.ServiceType.FullName}相同的实现类型。");

            AddDependency(dependency);
        }

        /// <summary>
        ///     添加依赖服务
        /// </summary>
        private void AddDependency(Dependency dependency)
        {
            lock (_sync)
            {
                Last.Next = dependency;
                Last = dependency.Last;
                Last.Last = dependency.Last;
            }
        }

        public IEnumerable<Dependency> Chain()
        {
            for (var @this = this; @this.Next != null; @this = @this.Next)
            {
                yield return @this;
            }
            yield return this.Last;
        }

        /// <summary>
        ///     重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"ServiceType:\"{ServiceType}\"  \nImplementationType:\"{GetImplementationType()}\"  \nLifetime:\"{Lifetime}\"\n";

        /// <summary>
        ///     创建实现类型的依赖服务
        /// </summary>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="lifetime">依赖服务生命周期</param>
        /// <param name="implementationType">依赖服务实现类型</param>
        /// <returns>依赖服务对象</returns>
        public static Dependency ForType(Type serviceType, DependencyLifetime lifetime, Type implementationType)
            => new Dependency(serviceType, lifetime, implementationType);

        /// <summary>
        ///     创建实例的依赖服务
        /// </summary>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="implementationInstance">依赖服务实例</param>
        /// <returns>依赖服务对象</returns>
        public static Dependency ForInstance(Type serviceType, object implementationInstance)
            => new Dependency(serviceType, implementationInstance);

        /// <summary>
        ///      创建委托的依赖服务
        /// </summary>
        /// <typeparam name="TService">依赖服务实现类型</typeparam>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="lifetime">依赖服务生命周期</param>
        /// <param name="implementationDelegate">返回依赖服务实现的委托</param>
        /// <returns>依赖服务对象</returns>
        public static Dependency ForDelegate<TService>(Type serviceType, DependencyLifetime lifetime,
            Func<IDependencyResolver, TService> implementationDelegate)
            where TService : class => new Dependency(serviceType, lifetime, implementationDelegate);
    }
}
