using System;

namespace FS.DI.Core
{
    /// <summary>
    /// 依赖入口
    /// </summary>
    public sealed class DependencyEntry
    {
        private readonly Object _sync = new Object();

        public DependencyStyle Style { get; internal set; }
        /// <summary>
        /// 依赖注入的实现类型
        /// </summary>
        internal Type ImplementationType { get; private set; }
        /// <summary>
        /// 依赖注入的实现类实例
        /// </summary>
        internal Object ImplementationInstance { get; private set; }
        /// <summary>
        /// 返回依赖注入实现类的委托
        /// </summary>
        internal Func<IDependencyResolver, object> ImplementationDelegate { get; private set; }
        /// <summary>
        /// 依赖注入的服务类型
        /// </summary>
        public Type ServiceType { get; private set; }
        /// <summary>
        /// 依赖注入类型的生命周期
        /// </summary>
        public DependencyLifetime Lifetime { get; internal set; }
        /// <summary>
        /// 依赖注入的下一个实现
        /// </summary>
        public DependencyEntry Next { get; private set; }
        /// <summary>
        /// 依赖注入的实现
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
        /// 返回依赖注入服务的实现类型
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
        /// 添加依赖注入的实现
        /// </summary>
        /// <param name="dependencyEntry"></param>
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

        private void AddEntry(DependencyEntry entry)
        {
            lock (_sync)
            {
                Last.Next = entry;
                Last = entry.Last;
                Last.Last = entry.Last;
            }
        }

        public override string ToString()
        {
            return String.Format("ServiceType:\"{0}\"  \nImplementationType:\"{1}\"  \nLifetime:\"{2}\"\n",
                ServiceType, GetImplementationType(), Lifetime);
        }

        /// <summary>
        /// 使用Type作为依赖注入的实现入口
        /// </summary>
        public static DependencyEntry ForType(Type serviceType, DependencyLifetime lifetime, Type implementationType)
        {
            return new DependencyEntry(serviceType, lifetime, implementationType);
        }

        /// <summary>
        /// 使用实例作为依赖注入的实现入口
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lifetime"></param>
        /// <param name="implementationInstance"></param>
        /// <returns></returns>
        public static DependencyEntry ForInstance(Type serviceType, Object implementationInstance)
        {
            return new DependencyEntry(serviceType, implementationInstance);
        }

        /// <summary>
        /// 使用委托作为依赖注入的实现入口
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceType"></param>
        /// <param name="lifetime"></param>
        /// <param name="implementationDelegate"></param>
        /// <returns></returns>
        public static DependencyEntry ForDelegate<TService>(Type serviceType, DependencyLifetime lifetime, Func<IDependencyResolver, TService> implementationDelegate)
            where TService : class
        {
            return new DependencyEntry(serviceType, lifetime, implementationDelegate);
        }
    }
}
