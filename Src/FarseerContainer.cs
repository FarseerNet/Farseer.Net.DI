using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FS.DI.Core
{
    /// <summary>
    ///  Farseer.IoC容器
    /// </summary>
    public sealed class FarseerContainer : IFarseerContainer, IDependencyRegisterProvider, IDependencyResolverProvider
    {
        private readonly Object _sync = new Object();
        /// <summary>
        /// 依赖注入对象的集合
        /// </summary>
        private readonly IDictionary<Type, DependencyEntry> _dependencyDictionary;

        private IDependencyRegisterProvider _dependencyRegisterProvider;
        private IDependencyResolverProvider _dependencyResolverProvider;
        /// <summary>
        /// 初始化IoC容器
        /// </summary>
        public FarseerContainer()
            : this(null)
        { }
        /// <summary>
        /// 初始化IoC容器
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
        /// 获取容器中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return _dependencyDictionary.Count;
            }
        }
        /// <summary>
        /// 将依赖注入对象添加到容器中
        /// </summary>
        /// <param name="dependencyEntry"></param>
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
        /// 深拷贝容器
        /// </summary>
        /// <returns>new IFarseerContainer</returns>
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
        /// <returns></returns>
        public IEnumerator<DependencyEntry> GetEnumerator()
        {
            return _dependencyDictionary.Values.GetEnumerator();
        }
        /// <summary>
        /// 返回一个循环访问容器的枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dependencyDictionary.GetEnumerator();
        }
        /// <summary>
        /// 从容器中移除所有对象
        /// </summary>
        public void Clear()
        {
            lock (_sync)
            {
                _dependencyDictionary.Clear();
            }
        }
        /// <summary>
        /// 创建服务注册器
        /// </summary>
        /// <returns></returns>
        public IDependencyRegister CreateRegister() => _dependencyRegisterProvider.CreateRegister();
        /// <summary>
        /// 创建服务注册器
        /// </summary>
        /// <returns></returns>
        IDependencyRegister IDependencyRegisterProvider.CreateRegister() => new Register.DependencyRegister(this);
        /// <summary>
        /// 创建服务解析器
        /// </summary>
        /// <returns></returns>
        public IDependencyResolver CreateResolver() => _dependencyResolverProvider.CreateResolver();

        /// <summary>
        /// 创建服务解析器
        /// </summary>
        /// <returns></returns>
        IDependencyResolver IDependencyResolverProvider.CreateResolver() => new Resolver.DependencyResolver(this);
        /// <summary>
        /// 设置服务注册器提供者
        /// </summary>
        /// <param name="dependencyRegisterProvider"></param>
        public void SetRegisterProvider(IDependencyRegisterProvider dependencyRegisterProvider)
        {
            if (dependencyRegisterProvider == null) throw new ArgumentNullException(nameof(dependencyRegisterProvider));
            _dependencyRegisterProvider = dependencyRegisterProvider;
        }
    }
}
