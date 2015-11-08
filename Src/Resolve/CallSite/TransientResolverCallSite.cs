using FS.DI.Core;
using FS.Extends;
using System;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    ///  Transient解析器调用
    /// </summary>
    internal sealed class TransientResolverCallSite : IResolverCallSite
    {
        private readonly IDependencyTable _dependencyTable;
        public TransientResolverCallSite(IDependencyTable dependencyTable)
        {
            if (dependencyTable == null) throw new ArgumentNullException(nameof(dependencyTable));
            _dependencyTable = dependencyTable;
        }
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.IsTransientLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            context.Handled = _dependencyTable.TryGetCompileValue(context, resolver);
        }
    }
}
