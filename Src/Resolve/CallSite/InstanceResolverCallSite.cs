﻿using System;
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
            Expression<Func<IDependencyResolver, object[], object>> factory =
                (r, args) =>
                    context.Dependency.ImplementationInstance;

            context.Resolved = factory;
        }
    }
}
