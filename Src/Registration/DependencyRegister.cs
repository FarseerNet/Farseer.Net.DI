using FS.DI.Core;
using System;

namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务注册器
    /// </summary>
    internal class DependencyRegister : IDependencyRegister
    {
        private readonly IFarseerContainer _container;
        internal DependencyRegister(IFarseerContainer container)
        {
            _container = container;
        }
        /// <summary>
        ///     注册依赖服务对象
        /// </summary>
        /// <param name="dependencyEntry">依赖服务对象</param>
        public void RegisterEntry(DependencyEntry dependencyEntry)
        {
            if (dependencyEntry == null)
                throw new ArgumentNullException(nameof(dependencyEntry));

            _container.Add(dependencyEntry);
        }   
    }
}
