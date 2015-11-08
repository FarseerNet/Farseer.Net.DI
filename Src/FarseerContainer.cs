using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FS.DI.Core
{
    /// <summary>
    ///     Farseer.IoC容器
    /// </summary>
    public sealed class FarseerContainer : IFarseerContainer, IDependencyRegisterProvider, IDependencyResolverProvider
    {
        private readonly Object _sync = new Object();
       
        private readonly IDictionary<Type, DependencyEntry> _dependencyDictionary;

        private IDependencyRegisterProvider _dependencyRegisterProvider;
        private IDependencyResolverProvider _dependencyResolverProvider;

        public FarseerContainer()
            : this(null)
        { }

        /// <summary>
        ///     初始化IoC容器
        /// </summary>
        /// <param name="dependencyEntrys"></param>
        public FarseerContainer(IEnumerable<DependencyEntry> dependencyEntrys)
        {
            _dependencyDictionary = new ConcurrentDictionary<Type, DependencyEntry>();
            if (dependencyEntrys != null)
            {
                foreach (var entry in dependencyEntrys)
                {
                    Add(entry);
                }
            }
            _dependencyRegisterProvider = this;
            _dependencyResolverProvider = this;
        }

        /// <summary>
        ///     获取容器中包含的依赖服务元素数
        /// </summary>
        public int Count
        {
            get
            {
                return _dependencyDictionary.Count;
            }
        }

        /// <summary>
        ///     添加依赖服务对象到容器中
        /// </summary>
        /// <param name="dependencyEntry">依赖服务对象</param>
        public void Add(DependencyEntry dependencyEntry)
        {
            if (dependencyEntry == null) throw new ArgumentNullException(nameof(dependencyEntry));
            lock (_sync)
            {
                var serviceType = dependencyEntry.ServiceType;
                if (_dependencyDictionary.ContainsKey(serviceType))
                {
                    _dependencyDictionary[serviceType].Add(dependencyEntry);
                }
                else
                {
                    _dependencyDictionary.Add(serviceType, dependencyEntry);
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
                var dependencyEntrys = _dependencyDictionary.Select(entry => entry.Value);
                return new FarseerContainer(dependencyEntrys);
            }
        }

        /// <summary>
        /// 返回一个循环访问容器的枚举器
        /// </summary>
        public IEnumerator<DependencyEntry> GetEnumerator()
        {
            return _dependencyDictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问容器的枚举器
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dependencyDictionary.GetEnumerator();
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
       
        public IDependencyRegister CreateRegister() => _dependencyRegisterProvider.CreateRegister();
       
        IDependencyRegister IDependencyRegisterProvider.CreateRegister() => new Registration.DependencyRegister(this);
       
        public IDependencyResolver CreateResolver() => _dependencyResolverProvider.CreateResolver();

      
        IDependencyResolver IDependencyResolverProvider.CreateResolver() => new Resolve.DependencyResolver(this);
       
        public void SetRegisterProvider(IDependencyRegisterProvider dependencyRegisterProvider)
        {
            if (dependencyRegisterProvider == null) throw new ArgumentNullException(nameof(dependencyRegisterProvider));
            _dependencyRegisterProvider = dependencyRegisterProvider;
        }
    }
}
