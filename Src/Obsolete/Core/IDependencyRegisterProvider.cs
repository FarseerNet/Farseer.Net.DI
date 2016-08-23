namespace FS.DI
{
    /// <summary>
    ///     依赖服务注册器提供者
    /// </summary>
    public interface IDependencyRegisterProvider
    {
        /// <summary>
        ///     创建依赖服务注册器
        /// </summary>
        /// <returns>依赖服务注册器</returns>
        IDependencyRegister CreateRegister();
    }
}
