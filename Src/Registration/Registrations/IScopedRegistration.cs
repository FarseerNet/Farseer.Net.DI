namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务作用域注册
    /// </summary>
    public interface IScopedRegistration<out TRegistration> 
    {
        /// <summary>
        ///     作为作用域生命周期的依赖服务
        /// </summary>      
        TRegistration AsScopedLifetime();
    }
}
