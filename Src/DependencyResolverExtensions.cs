using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.DI.Core
{
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// 解析服务
        /// </summary>
        public static TService Resolve<TService>(this IDependencyResolver dependencyResolver)
            where TService : class
        {
            if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));
            return (TService)dependencyResolver.Resolve(typeof(TService));
        }

        /// <summary>
        /// 解析服务集合
        /// </summary>
        public static IEnumerable<TService> ResolveAll<TService>(this IDependencyResolver dependencyResolver)
            where TService : class
        {
            if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));
            return dependencyResolver.ResolveAll(typeof(TService)).Select(t => (TService)t);
        }
    }
}
