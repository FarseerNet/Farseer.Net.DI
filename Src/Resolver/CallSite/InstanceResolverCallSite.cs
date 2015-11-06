using FS.DI.Core;
using FS.Extends;
using System;
using System.Linq.Expressions;
 
namespace FS.DI.Resolver.CallSite
{
    /// <summary>
    /// 实例解析器调用
    /// </summary>
    internal sealed class InstanceResolverCallSite : IResolverCallSite
    {
        public bool PreResolver(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.HasImplementationInstance();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            Expression<Func<IDependencyResolver, Object[], Object>> factory =
              (_r, _args) =>
              context.DependencyEntry.ImplementationInstance;

            context.CompleteValue = factory;
        }
    }
}
