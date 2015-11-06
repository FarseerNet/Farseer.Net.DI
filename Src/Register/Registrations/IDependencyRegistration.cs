namespace FS.DI.Register
{
    /// <summary>
    /// 依赖服务注册Configuration
    /// </summary>
    public interface IDependencyRegistration : ILifetimeRegistration<IDependencyRegistration>, IDynamicProxyRegistration<IDependencyRegistration>
    {
    }
}
