﻿using FS.Cache;
using FS.DI.Core;
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
            return context.NotResolved() && context.Resolved == null && context.HasImplementationType() && !context.HasPublicConstructor();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            context.Resolved = Expression.Lambda<Func<IDependencyResolver, Object[], Object>>(
                Expression.New(context.IsDynamicProxy() ?
                DynamicTypeCacheManager.GetCache(context.DependencyEntry.ImplementationType) :
                context.DependencyEntry.ImplementationType),
                Expression.Parameter(typeof(IDependencyResolver)),
                Expression.Parameter(typeof(Object[])));
        }
    }
}