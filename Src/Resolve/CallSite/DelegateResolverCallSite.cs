using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 委托解析器调用
    /// </summary>
    internal sealed class DelegateResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.Resolved == null && context.HasImplementationDelegate();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            Expression<Func<IDependencyResolver, object[], object>> factory =
                (r, args) =>
                    context.Dependency.ImplementationDelegate(r);

            context.Resolved = factory;
        }
    }
}
