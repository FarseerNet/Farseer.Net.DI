using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务集合注册
    /// </summary>
    internal sealed class EnumerableRegistration : IEnumerableRegistration
    {
        /// <summary>
        ///     依赖服务注册集合
        /// </summary>
        private readonly ICollection<IDependencyRegistration> _configurationCollection;

        public EnumerableRegistration(IEnumerable<IDependencyRegistration> configurationCollection)
        {
            if (configurationCollection == null) throw new ArgumentNullException(nameof(configurationCollection));
            _configurationCollection = new List<IDependencyRegistration>(configurationCollection);
        }

        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public IEnumerableRegistration AsDynamicProxy()
        {
            foreach (var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsDynamicProxy();
            }
            return this;
        }

        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public IEnumerableRegistration AsPropertyInjection()
        {
            foreach (var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsPropertyInjection();
            }
            return this;
        }

        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>  
        public IEnumerableRegistration AsScopedLifetime()
        {
            foreach (var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsScopedLifetime();
            }
            return this;
        }

        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public IEnumerableRegistration AsSingletonLifetime()
        {
            foreach (var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsSingletonLifetime();
            }
            return this;
        }

        /// <summary>
        ///        作为瞬态实例生命周期的依赖服务
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public IEnumerableRegistration AsTransientLifetime()
        {
            foreach (var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsTransientLifetime();
            }
            return this;
        }

        public IEnumerator<IDependencyRegistration> GetEnumerator()
            => _configurationCollection.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
