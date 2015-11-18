using FS.DI.Core;
using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 实例解析器调用
    /// </summary>
    internal sealed class InstanceResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.Resolved == null && context.HasImplementationInstance();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            Expression<Func<IDependencyResolver, Object[], Object>> factory =
              (_r, _args) =>
              context.DependencyEntry.ImplementationInstance;

            context.Resolved = factory;
        }
    }
}
