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
        /// <param name="dependency">依赖服务对象</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterDependency(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));

            _container.Add(dependency);
        }
    }
}
