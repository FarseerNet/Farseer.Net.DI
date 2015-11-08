using FS.DI.Core;
using System;

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
        Boolean Handled { get; set; }

        /// <summary>
        /// 上下文对象
        /// </summary>
        Object Value { get; set; }

        /// <summary>
        /// DependencyEntry
        /// </summary>
        DependencyEntry DependencyEntry { get; }

    }
}
 