namespace FS.DI.Register
{
    /// <summary>
    /// 依赖注入对象的生命周期配置
    /// </summary>
    /// <typeparam name="TRegistration"></typeparam>
    public interface ILifetimeRegistration<out TRegistration> : ITransientRegistration<TRegistration>, IScopedRegistration<TRegistration>, ISingletonRegistration<TRegistration>, IPropertyRegistration<TRegistration>
    {
    }
    public interface ILifetimeRegistration : ILifetimeRegistration<ILifetimeRegistration>
    {
    }
}