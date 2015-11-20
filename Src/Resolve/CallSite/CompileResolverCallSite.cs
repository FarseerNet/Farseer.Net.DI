﻿using FS.Cache;
using FS.DI.Core;
using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 编译解析器
    /// </summary>
    internal sealed class CompileResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved() && context.Resolved is Expression;
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            try
            {
                var args = context.HasImplementationType() && context.HasPublicConstructor() ?
                     ResolverHelper.GetConstructorParameters(
                     context.DependencyEntry.GetImplementationType(), resolver) : new Object[] { };
                context.Resolved = CompileCacheManager.GetOrSetCache(context.DependencyEntry,
                    () => CreateDelegate(context.Resolved as Expression)).
                    Invoke(resolver, args);
                CacheResolved(context, resolver);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("未能创建类型\"{0}\"的实例。", context.DependencyEntry.ServiceType), ex);
            }
        }

        /// <summary>
        /// 编译表达式树生成委托
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Func<IDependencyResolver, Object[], Object> CreateDelegate(Expression body)
        {
            return (body as Expression<Func<IDependencyResolver, Object[], Object>>).Compile();
        }

        private void CacheResolved(IResolverContext context, IDependencyResolver resolver)
        {
            if (context.IsSingletonLifetime())
            {
                ScopedCacheManager.SetCache(((DependencyResolver)resolver).RootScoped, context.DependencyEntry, context.Resolved);
            }
            if (context.IsScopedLifetime())
            {
                ScopedCacheManager.SetCache((IScopedResolver)resolver, context.DependencyEntry, context.Resolved);
            }
        }
    }
}
