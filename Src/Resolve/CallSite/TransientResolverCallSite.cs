using FS.Cache;
using FS.DI.Core;
using System;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    ///  Transient解析器调用
    /// </summary>
    internal sealed class TransientResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.IsTransientLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            var args = context.HasImplementationType() && context.HasPublicConstructor() ?
                ResolverHelper.GetConstructorParameters(
                    context.DependencyEntry.GetImplementationType(), resolver) : new Object[] { };
            context.Resolved = CompileCacheManager.GetCache(context.DependencyEntry, resolver, args);
            context.Handled = context.Resolved != null && !PropertyCacheManager.Any(context.DependencyEntry);
        }
    }
}
