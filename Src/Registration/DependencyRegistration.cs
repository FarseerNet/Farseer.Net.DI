using FS.DI.Core;

namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务注册
    /// </summary>
    internal sealed class DependencyRegistration : IDependencyRegistration, ILifetimeRegistration, ISingletonRegistration
    {
        internal DependencyEntry Entry { get; set; }

        public DependencyRegistration(DependencyEntry entry)
        {
            Entry = entry;
        }

        #region IDependencyRegistration

        /// <summary>
        ///        作为瞬态实例生命周期的依赖服务
        /// </summary>
        public IDependencyRegistration AsTransientLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Transient;
            return this;
        }

        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>  
        public IDependencyRegistration AsScopedLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Scoped;
            return this;
        }

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        public IDependencyRegistration AsSingletonLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Singleton;
            return this;
        }

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        public IDependencyRegistration AsPropertyInjection()
        {
            if (!Entry.Style.HasFlag(DependencyStyle.PropertyInjection))
            {
                Entry.Style = Entry.Style | DependencyStyle.PropertyInjection;
            }
            return this;
        }

        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        public IDependencyRegistration AsDynamicProxy()
        {
            if (!Entry.Style.HasFlag(DependencyStyle.DynamicProxy))
            {
                Entry.Style = Entry.Style | DependencyStyle.DynamicProxy;   
            }
            return this;
        }

        #endregion

        #region ILifetimeRegistration

        /// <summary>
        ///        作为瞬态实例生命周期的依赖服务
        /// </summary>
        ILifetimeRegistration ITransientRegistration<ILifetimeRegistration>.AsTransientLifetime()
        {
            return (DependencyRegistration)AsTransientLifetime();
        }

        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>  
        ILifetimeRegistration IScopedRegistration<ILifetimeRegistration>.AsScopedLifetime()
        {
            return (DependencyRegistration)AsScopedLifetime();
        }

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        ILifetimeRegistration ISingletonRegistration<ILifetimeRegistration>.AsSingletonLifetime()
        {
            return (DependencyRegistration)AsSingletonLifetime();
        }

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        ILifetimeRegistration IPropertyRegistration<ILifetimeRegistration>.AsPropertyInjection()
        {
            return (DependencyRegistration)AsPropertyInjection();
        }

        #endregion

        #region ISingletonRegistration

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        ISingletonRegistration ISingletonRegistration<ISingletonRegistration>.AsSingletonLifetime()
        {
            return (DependencyRegistration)AsSingletonLifetime();
        }

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        ISingletonRegistration IPropertyRegistration<ISingletonRegistration>.AsPropertyInjection()
        {
            return (DependencyRegistration)AsPropertyInjection();
        }

        #endregion
    }
}
