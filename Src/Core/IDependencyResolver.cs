using FS.DI.Resolve;
using System;
using System.Collections.Generic;

namespace FS.DI.Core
{
    /// <summary>
    ///     依赖服务解析器
    /// </summary>
    public interface IDependencyResolver : IServiceProvider, IScopedResolverProvider, IDisposable
    {

        /// <summary>
        ///     解析依赖服务
        /// </summary>
        Object Resolve(Type serviceType);

        /// <summary>
        ///     解析依赖服务
        /// </summary>
        IEnumerable<Object> ResolveAll(Type serviceType);
    } 
}
