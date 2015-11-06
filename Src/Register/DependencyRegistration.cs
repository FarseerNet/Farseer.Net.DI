using FS.Cache;
using FS.DI.Core;

namespace FS.DI.Register
{
    /// <summary>
    /// 依赖服务注册实现类
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
        /// 注册为瞬态实例的生命周期
        /// </summary>
        /// <returns></returns>
        public IDependencyRegistration AsTransientLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Transient;
            return this;
        }

        /// <summary>
        /// 注册为作用域的生命周期
        /// </summary>
        /// <returns></returns>
        public IDependencyRegistration AsScopedLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// 注册为单例的生命周期
        /// </summary>
        /// <returns></returns>
        public IDependencyRegistration AsSingletonLifetime()
        {
            Entry.Lifetime = DependencyLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// 注册为属性依赖
        /// </summary>
        public IDependencyRegistration AsPropertyInjection()
        {
            if (!Entry.Style.HasFlag(DependencyStyle.PropertyInjection))
            {
                Entry.Style = Entry.Style | DependencyStyle.PropertyInjection;
            }
            return this;
        }

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

        ILifetimeRegistration ITransientRegistration<ILifetimeRegistration>.AsTransientLifetime()
        {
            return (DependencyRegistration)AsTransientLifetime();
        }

        ILifetimeRegistration IScopedRegistration<ILifetimeRegistration>.AsScopedLifetime()
        {
            return (DependencyRegistration)AsScopedLifetime();
        }

        ILifetimeRegistration ISingletonRegistration<ILifetimeRegistration>.AsSingletonLifetime()
        {
            return (DependencyRegistration)AsSingletonLifetime();
        }

        ILifetimeRegistration IPropertyRegistration<ILifetimeRegistration>.AsPropertyInjection()
        {
            return (DependencyRegistration)AsPropertyInjection();
        }

        #endregion

        #region ISingletonRegistration

        /// <summary>
        /// 注册为单例的生命周期
        /// </summary>
        /// <returns></returns>
        ISingletonRegistration ISingletonRegistration<ISingletonRegistration>.AsSingletonLifetime()
        {
            return (DependencyRegistration)AsSingletonLifetime();
        }

        /// <summary>
        /// 注册为属性依赖
        /// </summary>
        ISingletonRegistration IPropertyRegistration<ISingletonRegistration>.AsPropertyInjection()
        {
            return (DependencyRegistration)AsPropertyInjection();
        }

        #endregion
    }
}
