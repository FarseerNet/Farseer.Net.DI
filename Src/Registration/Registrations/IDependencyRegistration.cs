namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务注册
    /// </summary>
    public interface IDependencyRegistration : ILifetimeRegistration<IDependencyRegistration>, IDynamicProxyRegistration<IDependencyRegistration>
    {
    }
}
