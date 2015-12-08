namespace FS.DI
{
    /// <summary>
    ///     依赖服务生命周期
    /// </summary>
    public enum DependencyLifetime
    {
        /// <summary>
        ///     单例
        /// </summary>
        Singleton,
        /// <summary>
        ///     作用域
        /// </summary>
        Scoped,
        /// <summary>
        ///     瞬态实例
        /// </summary>
        Transient
    }
}
