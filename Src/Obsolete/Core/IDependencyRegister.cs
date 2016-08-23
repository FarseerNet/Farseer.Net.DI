namespace FS.DI
{
    /// <summary>
    ///     依赖服务注册器
    /// </summary>
    public interface IDependencyRegister
    {
        /// <summary>
        ///     注册依赖服务对象
        /// </summary>
        void RegisterDependency(Dependency dependency);
    }
}
