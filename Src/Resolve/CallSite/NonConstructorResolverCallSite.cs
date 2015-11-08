using FS.Cache;
using FS.DI.Core;
using FS.Extends;
using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 无构造方法解析器调用
    /// </summary>
    internal sealed class NonConstructorResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.HasImplementationType() && !context.HasPublicConstructor();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            //var implType = context.DependencyEntry.ImplementationType;
            var implType = DynamicTypeCacheManager.Cache(context.DependencyEntry.ImplementationType);
            var body = Expression.New(implType);
            var factory = Expression.Lambda<Func<IDependencyResolver, Object[], Object>>(body,
                Expression.Parameter(typeof(IDependencyResolver)),
                Expression.Parameter(typeof(Object[])));
            context.Value = factory;
        }
    }
}
