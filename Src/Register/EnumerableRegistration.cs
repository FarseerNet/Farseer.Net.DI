using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FS.DI.Register
{
    /// <summary>
    /// 注册方式为集合的依赖注入对象的描述
    /// </summary>
    internal sealed class EnumerableRegistration : IEnumerableRegistration
    {
        /// <summary>
        /// 依赖注入对象集合
        /// </summary>
        private ICollection<IDependencyRegistration> _configurationCollection;

        public EnumerableRegistration(IEnumerable<IDependencyRegistration> configurationCollection)
        {
            if (configurationCollection == null) throw new ArgumentNullException(nameof(configurationCollection));
            _configurationCollection = new List<IDependencyRegistration>(configurationCollection);
        }

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
        /// 注册为作用域的生命周期
        /// </summary>
        /// <returns></returns>
        public IEnumerableRegistration AsScopedLifetime()
        {
            foreach(var configuration in _configurationCollection)
            {
                if (configuration == null)
                    throw new NullReferenceException("IDependencyConfiguration不能为null");

                configuration.AsScopedLifetime();
            }
            return this;
        }
        /// <summary>
        /// 注册为单例的生命周期
        /// </summary>
        /// <returns></returns>
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
        /// 注册为瞬态实例的生命周期
        /// </summary>
        /// <returns></returns>
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
        {
            return _configurationCollection.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
