using FS.Cache;
using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 无构造方法解析器调用
    /// </summary>
    internal sealed class NewResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.Resolved == null && context.HasImplementationType() &&
                   !context.HasPublicConstructor();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            context.Resolved = Expression.Lambda<Func<IDependencyResolver, object[], object>>(
                    Expression.New(context.IsDynamicProxy()
                    ? DynamicTypeCacheManager.GetCache(context.Dependency.ImplementationType)
                    : context.Dependency.ImplementationType),
                    Expression.Parameter(typeof (IDependencyResolver)),
                    Expression.Parameter(typeof (object[])));
        }
    }
}
