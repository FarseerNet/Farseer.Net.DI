﻿using FS.Cache;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    ///  Singleton解析器调用
    /// </summary>
    internal sealed class SingletonResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.IsSingletonLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            var single = ScopedCacheManager.GetCache(((DependencyResolver) resolver).RootScoped, context.Dependency);
            context.Resolved = single;
            context.Handled = single != null;
        }
    }
}
