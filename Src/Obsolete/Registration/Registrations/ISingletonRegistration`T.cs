namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖注入单例注册
    /// </summary>
    public interface ISingletonRegistration<out TRegistration> : IPropertyRegistration<TRegistration>
    {
        /// <summary>
        ///     作为单例生命周期的依赖服务
        /// </summary>
        TRegistration AsSingletonLifetime();
    }
}
