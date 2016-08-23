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

        public Dependency Dependency { get; }

        public ResolverContext(Dependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            Dependency = dependency;
        }
    }
}
