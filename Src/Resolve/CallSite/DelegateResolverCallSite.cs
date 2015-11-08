using FS.DI.Core;
using FS.Extends;
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
            return context.NotComplete() && context.HasImplementationDelegate();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {           
            Expression<Func<IDependencyResolver, Object[], Object>> factory =
                (_r, _args) =>
                context.DependencyEntry.ImplementationDelegate(_r);

            context.Value = factory;
        }
    }
}
