using FS.Cache;
using FS.DI.Core;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    ///  Scoped解析器调用
    /// </summary>
    internal sealed class ScopedResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.IsScopedLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            var single = ScopedCacheManager.GetCache((IScopedResolver)resolver, context.DependencyEntry);
            context.Resolved = single;
            context.Handled = single != null;
        }
    }
}
