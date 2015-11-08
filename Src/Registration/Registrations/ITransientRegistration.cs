namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务瞬时实例注册
    /// </summary>
    public interface ITransientRegistration<out TRegistration>
    {
        /// <summary>
        ///    作为瞬态实例生命周期的依赖服务
        /// </summary>
        TRegistration AsTransientLifetime();
    }
}
