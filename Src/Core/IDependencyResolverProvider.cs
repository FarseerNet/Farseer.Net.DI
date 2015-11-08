namespace FS.DI.Core
{
    /// <summary>
    ///     依赖服务解析器提供者
    /// </summary>
    public interface IDependencyResolverProvider
    {
        /// <summary>
        ///     创建依赖服务解析器
        /// </summary>
        /// <returns>依赖服务解析器</returns>
        IDependencyResolver CreateResolver();
    }
}
