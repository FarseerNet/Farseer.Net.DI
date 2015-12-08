using System;
using System.Collections.Generic;

namespace FS.DI
{
    /// <summary>
    ///     依赖服务解析器
    /// </summary>
    public interface IDependencyResolver : IServiceProvider, IScopedResolverProvider, IDisposable
    {

        /// <summary>
        ///     解析依赖服务
        /// </summary>
        object Resolve(Type serviceType);

        /// <summary>
        ///     解析依赖服务
        /// </summary>
        IEnumerable<object> ResolveAll(Type serviceType);
    } 
}
