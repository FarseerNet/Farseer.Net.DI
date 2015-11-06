﻿using FS.DI.Core;
using FS.Extends;
using System;

namespace FS.DI.Resolver.CallSite
{
    /// <summary>
    ///  Scoped解析器调用
    /// </summary>
    internal sealed class ScopedResolverCallSite : IResolverCallSite
    {
        private readonly IDependencyTable _dependencyTable;
        public ScopedResolverCallSite(IDependencyTable dependencyTable)
        {
            if (dependencyTable == null) throw new ArgumentNullException(nameof(dependencyTable));
            _dependencyTable = dependencyTable;
        }
        public bool PreResolver(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.IsScopedLifetime();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            Object completeValue;
            if (_dependencyTable.TryGetScoped(context, resolver, out completeValue))
            {
                context.CompleteValue = completeValue;
                context.CompleteHandled = true;
            }
        }
    }
}
