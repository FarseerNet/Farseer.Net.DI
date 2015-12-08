namespace FS.DI
{
    /// <summary>
    ///     作用域服务解析器提供者
    /// </summary>
    public interface IScopedResolverProvider
    {
        /// <summary>
        ///     创建作用域服务解析器
        /// </summary>
        IScopedResolver CreateScopedResolver();
    } 
}
