using FS.DI.Core;
using System;

namespace FS.DI.Resolve
{
    /// <summary>
    /// 解析器上下文实现
    /// </summary>
    internal class ResolverContext : IResolverContext
    {
        public bool Handled { get; set; } = false;

        public object Resolved { get; set; }
        public DependencyEntry DependencyEntry { get; private set; }

        public ResolverContext(DependencyEntry dependencyEntry)
        {
            if (dependencyEntry == null) throw new ArgumentNullException(nameof(dependencyEntry));
            DependencyEntry = dependencyEntry;
        }
    }
}
