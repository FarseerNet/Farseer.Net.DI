
namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务注册
    /// </summary>
    internal sealed class DependencyRegistration : IDependencyRegistration, ILifetimeRegistration,
        ISingletonRegistration
    {
        internal Dependency Dependency { get; }

        public DependencyRegistration(Dependency dependency)
        {
            Dependency = dependency;
        }

        #region IDependencyRegistration

        /// <summary>
        ///        作为瞬态实例生命周期的依赖服务
        /// </summary>
        public IDependencyRegistration AsTransientLifetime()
        {
            Dependency.Lifetime = DependencyLifetime.Transient;
            return this;
        }

        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>  
        public IDependencyRegistration AsScopedLifetime()
        {
            Dependency.Lifetime = DependencyLifetime.Scoped;
            return this;
        }

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        public IDependencyRegistration AsSingletonLifetime()
        {
            Dependency.Lifetime = DependencyLifetime.Singleton;
            return this;
        }

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        public IDependencyRegistration AsPropertyInjection()
        {
            if (!Dependency.Style.HasFlag(DependencyStyle.PropertyInjection))
            {
                Dependency.Style = Dependency.Style | DependencyStyle.PropertyInjection;
            }
            return this;
        }

        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        public IDependencyRegistration AsDynamicProxy()
        {
            if (!Dependency.Style.HasFlag(DependencyStyle.DynamicProxy))
            {
                Dependency.Style = Dependency.Style | DependencyStyle.DynamicProxy;
            }
            return this;
        }

        #endregion

        #region ILifetimeRegistration

        /// <summary>
        ///        作为瞬态实例生命周期的依赖服务
        /// </summary>
        ILifetimeRegistration ITransientRegistration<ILifetimeRegistration>.AsTransientLifetime()
            => (DependencyRegistration) AsTransientLifetime();

        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>  
        ILifetimeRegistration IScopedRegistration<ILifetimeRegistration>.AsScopedLifetime()
            => (DependencyRegistration) AsScopedLifetime();

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        ILifetimeRegistration ISingletonRegistration<ILifetimeRegistration>.AsSingletonLifetime()
            => (DependencyRegistration) AsSingletonLifetime();

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        ILifetimeRegistration IPropertyRegistration<ILifetimeRegistration>.AsPropertyInjection()
            => (DependencyRegistration) AsPropertyInjection();

        #endregion

        #region ISingletonRegistration

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        ISingletonRegistration ISingletonRegistration<ISingletonRegistration>.AsSingletonLifetime()
            => (DependencyRegistration) AsSingletonLifetime();

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        ISingletonRegistration IPropertyRegistration<ISingletonRegistration>.AsPropertyInjection()
            => (DependencyRegistration) AsPropertyInjection();

        #endregion
    }
}
