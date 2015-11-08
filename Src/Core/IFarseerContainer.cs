using System.Collections;
using System.Collections.Generic;

namespace FS.DI.Core
{
    /// <summary>
    ///     Farseer.IoC容器
    /// </summary>
    public interface IFarseerContainer : IEnumerable<DependencyEntry>, IDependencyRegisterProvider, IDependencyResolverProvider, IEnumerable
    {
        /// <summary>
        ///     获取容器中包含的依赖服务元素数
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     添加依赖服务对象到容器中
        /// </summary>
        /// <param name="dependencyEntry">依赖服务对象</param>
        void Add(DependencyEntry dependencyEntry);

        /// <summary>
        ///     深拷贝容器
        /// </summary>
        IFarseerContainer Clone();

        /// <summary>
        /// 从容器中移除所有依赖服务对象
        /// </summary>
        void Clear();

        /// <summary>
        ///     设置依赖服务注册器提供者
        /// </summary>
        /// <param name="registerProvider">依赖服务注册器提供者</param>
        void SetRegisterProvider(IDependencyRegisterProvider registerProvider);
    }
}
