using FS.DI.Core;
using FS.Extends;
using System;

namespace FS.DI.Resolver.CallSite
{
    /// <summary>
    ///  Singleton解析器调用
    /// </summary>
    internal sealed class SingletonResolverCallSite : IResolverCallSite
    {
        private readonly IDependencyTable _dependencyTable;
        public SingletonResolverCallSite(IDependencyTable dependencyTable)
        {
            if (dependencyTable == null) throw new ArgumentNullException(nameof(dependencyTable));
            _dependencyTable = dependencyTable;
        }
        public bool PreResolver(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.IsSingletonLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            Object completeValue;
            if (_dependencyTable.TryGetScoped(context, null, out completeValue))
            {
                context.CompleteValue = completeValue;
                context.CompleteHandled = true;
            }        
        }
    }
}
