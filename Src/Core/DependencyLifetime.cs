namespace FS.DI.Core
{
    /// <summary>
    /// 指示依赖注入对象的生命周期
    /// </summary>
    public enum DependencyLifetime
    {
        /// <summary>
        /// 表示为单例的生命周期
        /// </summary>
        Singleton,
        /// <summary>
        /// 表示为作用域的生命周期
        /// </summary>
        Scoped,
        /// <summary>
        /// 表示为瞬态实例的生命周期
        /// </summary>
        Transient
    }
}
