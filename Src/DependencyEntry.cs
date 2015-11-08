using System;

namespace FS.DI.Core
{
    /// <summary>
    ///     依赖服务对象
    /// </summary>
    public sealed class DependencyEntry
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private readonly Object _sync = new Object();

        /// <summary>
        ///     依赖服务风格
        /// </summary>
        public DependencyStyle Style { get; internal set; }

        /// <summary>
        ///     依赖服务实现类型
        /// </summary>
        internal Type ImplementationType { get; private set; }

        /// <summary>
        ///     依赖服务实例
        /// </summary>
        internal Object ImplementationInstance { get; private set; }

        /// <summary>
        ///     返回依赖服务实现类委托
        /// </summary>
        internal Func<IDependencyResolver, object> ImplementationDelegate { get; private set; }

        /// <summary>
        ///     依赖服务类型
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        ///     依赖服务生命周期
        /// </summary>
        public DependencyLifetime Lifetime { get; internal set; }

        /// <summary>
        ///     依赖服务下一个实现
        /// </summary>
        public DependencyEntry Next { get; private set; }

        /// <summary>
        ///     依赖服务实现
        /// </summary>
        public DependencyEntry Last { get; private set; }

        private DependencyEntry(Type serviceType, DependencyLifetime lifetime)
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

        private DependencyEntry(Type serviceType, DependencyLifetime lifetime, Type implementationType)
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
                throw new InvalidOperationException(String.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName, implementationType.FullName));
            }

            ImplementationType = implementationType;
        }

        private DependencyEntry(Type serviceType, Object implementationInstance)
            : this(serviceType, DependencyLifetime.Singleton)
        {
            var implType = implementationInstance.GetType();

            if (!serviceType.IsAssignableFrom(implType))
            {
                throw new InvalidOperationException(String.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName, implType.FullName));
            }

            ImplementationInstance = implementationInstance;
        }

        private DependencyEntry(Type serviceType, DependencyLifetime lifetime, Func<IDependencyResolver, object> implementationDelegate)
            : this(serviceType, lifetime)
        {
            ImplementationDelegate = implementationDelegate;

            var implType = GetDelegateReturnType();

            if (!serviceType.IsAssignableFrom(implType))
                throw new InvalidOperationException(String.Format("无法由类型\"{1}\"创建\"{0}\"的实例。", serviceType.FullName, implType.FullName));
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

            throw new InvalidOperationException(string.Format("无法获取{0}的实现类型。", ServiceType.FullName));
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
        /// <param name="dependencyEntry">相同类型的依赖服务</param>
        public void Add(DependencyEntry dependencyEntry)
        {
            if (dependencyEntry == null)
            {
                throw new ArgumentNullException(nameof(dependencyEntry));
            }

            if (ServiceType != dependencyEntry.ServiceType)
            {
                throw new ArgumentOutOfRangeException(nameof(dependencyEntry), "当前注册的服务类型需于目标服务类型一致。");
            }

            if (GetImplementationType() == dependencyEntry.GetImplementationType())
            {
                throw new ArgumentOutOfRangeException(nameof(dependencyEntry), "已注册" + dependencyEntry.ServiceType.FullName + "相同的实现类型。");
            }

            AddEntry(dependencyEntry);
        }

        /// <summary>
        ///     添加依赖服务
        /// </summary>
        private void AddEntry(DependencyEntry entry)
        {
            lock (_sync)
            {
                Last.Next = entry;
                Last = entry.Last;
                Last.Last = entry.Last;
            }
        }

        /// <summary>
        ///     重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ServiceType:\"{0}\"  \nImplementationType:\"{1}\"  \nLifetime:\"{2}\"\n",
                ServiceType, GetImplementationType(), Lifetime);
        }

        /// <summary>
        ///     创建实现类型的依赖服务
        /// </summary>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="lifetime">依赖服务生命周期</param>
        /// <param name="implementationType">依赖服务实现类型</param>
        /// <returns>依赖服务对象</returns>
        public static DependencyEntry ForType(Type serviceType, DependencyLifetime lifetime, Type implementationType)
        {
            return new DependencyEntry(serviceType, lifetime, implementationType);
        }

        /// <summary>
        ///     创建实例的依赖服务
        /// </summary>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="implementationInstance">依赖服务实例</param>
        /// <returns>依赖服务对象</returns>
        public static DependencyEntry ForInstance(Type serviceType, Object implementationInstance)
        {
            return new DependencyEntry(serviceType, implementationInstance);
        }

        /// <summary>
        ///      创建委托的依赖服务
        /// </summary>
        /// <typeparam name="TService">依赖服务实现类型</typeparam>
        /// <param name="serviceType">依赖服务类型</param>
        /// <param name="lifetime">依赖服务生命周期</param>
        /// <param name="implementationDelegate">返回依赖服务实现的委托</param>
        /// <returns>依赖服务对象</returns>
        public static DependencyEntry ForDelegate<TService>(Type serviceType, DependencyLifetime lifetime, Func<IDependencyResolver, TService> implementationDelegate)
            where TService : class
        {
            return new DependencyEntry(serviceType, lifetime, implementationDelegate);
        }
    }
}
