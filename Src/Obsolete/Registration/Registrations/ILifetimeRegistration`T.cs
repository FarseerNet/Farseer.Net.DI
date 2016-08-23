namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务生命周期注册
    /// </summary>
    public interface ILifetimeRegistration<out TRegistration> : ITransientRegistration<TRegistration>, IScopedRegistration<TRegistration>, ISingletonRegistration<TRegistration>, IPropertyRegistration<TRegistration>
    {
    }
}
