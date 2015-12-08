using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FS.DI
{
    /// <summary>
    ///     Farseer.IoC容器
    /// </summary>
    public sealed class FarseerContainer : IFarseerContainer
    {
        private readonly object _sync = new object();

        private readonly IDictionary<Type, Dependency> _dependencyDictionary;

        private IDependencyRegisterProvider _dependencyRegisterProvider;
        private readonly IDependencyResolverProvider _dependencyResolverProvider;

        public FarseerContainer()
            : this(null)
        {
        }

        /// <summary>
        ///     初始化IoC容器
        /// </summary>
        /// <param name="dependencys"></param>
        // ReSharper disable once MemberCanBePrivate.Global
        public FarseerContainer(IEnumerable<Dependency> dependencys)
        {
            _dependencyDictionary = new ConcurrentDictionary<Type, Dependency>();
            if (dependencys != null)
                foreach (var dependency in dependencys)
                    Add(dependency);
            _dependencyRegisterProvider = this;
            _dependencyResolverProvider = this;
        }

        /// <summary>
        ///     获取容器中包含的依赖服务元素数
        /// </summary>
        public int Count => _dependencyDictionary.Count;

        /// <summary>
        ///     添加依赖服务对象到容器中
        /// </summary>
        /// <param name="dependency">依赖服务对象</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(Dependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            lock (_sync)
            {
                var serviceType = dependency.ServiceType;
                if (_dependencyDictionary.ContainsKey(serviceType))
                {
                    _dependencyDictionary[serviceType].Add(dependency);
                }
                else
                {
                    _dependencyDictionary.Add(serviceType, dependency);
                }
            }
        }

        /// <summary>
        ///     深拷贝容器
        /// </summary>
        public IFarseerContainer Clone()
        {
            lock (_sync)
            {
                var dependencys = _dependencyDictionary.Select(dependency => dependency.Value);
                return new FarseerContainer(dependencys);
            }
        }

        /// <summary>
        /// 返回一个循环访问容器的枚举器
        /// </summary>
        public IEnumerator<Dependency> GetEnumerator()
        {
            return _dependencyDictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问容器的枚举器
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_sync)
            {
                return _dependencyDictionary.GetEnumerator();
            }
        }

        /// <summary>
        /// 从容器中移除所有依赖服务对象
        /// </summary>
        public void Clear()
        {
            lock (_sync)
            {
                _dependencyDictionary.Clear();
            }
        }

        /// <summary>
        ///     创建依赖服务注册器
        /// </summary>
        /// <returns>依赖服务注册器</returns>
        public IDependencyRegister CreateRegister() => _dependencyRegisterProvider.CreateRegister();

        /// <summary>
        ///     创建依赖服务注册器
        /// </summary>
        /// <returns>依赖服务注册器</returns>
        IDependencyRegister IDependencyRegisterProvider.CreateRegister() => new Registration.DependencyRegister(this);

        /// <summary>
        ///     创建依赖服务解析器
        /// </summary>
        /// <returns>依赖服务解析器</returns>
        public IDependencyResolver CreateResolver() => _dependencyResolverProvider.CreateResolver();

        /// <summary>
        ///     创建依赖服务解析器
        /// </summary>
        /// <returns>依赖服务解析器</returns>
        IDependencyResolver IDependencyResolverProvider.CreateResolver() => new Resolve.DependencyResolver(this);

        /// <summary>
        ///     设置依赖服务注册器提供者
        /// </summary>
        /// <param name="dependencyRegisterProvider">依赖服务注册器提供者</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetRegisterProvider(IDependencyRegisterProvider dependencyRegisterProvider)
        {
            if (dependencyRegisterProvider == null) throw new ArgumentNullException(nameof(dependencyRegisterProvider));
            _dependencyRegisterProvider = dependencyRegisterProvider;
        }

        public void Dispose()
        {
            foreach (var dependency in _dependencyDictionary.Values)
            {
                var disposable = dependency.ImplementationInstance as IDisposable;
                disposable?.Dispose();
                Cache.CompileCacheManager.RemoveCache(dependency);
            }
            this.Clear();
        }
    }
}
