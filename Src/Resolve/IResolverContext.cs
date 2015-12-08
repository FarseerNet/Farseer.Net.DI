
namespace FS.DI.Resolve
{
    /// <summary>
    /// 解析器上下文
    /// </summary>
    public interface IResolverContext
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool Handled { get; set; }

        /// <summary>
        /// 上下文对象
        /// </summary>
        object Resolved { get; set; }

        /// <summary>
        /// Dependency
        /// </summary>
        Dependency Dependency { get; }

    }
}
 