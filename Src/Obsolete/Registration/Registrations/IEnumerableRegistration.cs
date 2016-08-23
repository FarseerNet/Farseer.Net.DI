using System.Collections;
using System.Collections.Generic;

namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务集合注册
    /// </summary>
    public interface IEnumerableRegistration : IEnumerable<IDependencyRegistration>, ILifetimeRegistration<IEnumerableRegistration>, IDynamicProxyRegistration<IEnumerableRegistration>, IEnumerable
    {
    }
}
