using System.Collections;
using System.Collections.Generic;

namespace FS.DI.Register
{
    /// <summary>
    /// 依赖服务集合注册Configuration
    /// </summary>
    public interface IEnumerableRegistration : IEnumerable<IDependencyRegistration>, ILifetimeRegistration<IEnumerableRegistration>, IDynamicProxyRegistration<IEnumerableRegistration>, IEnumerable
    {
    }
}
