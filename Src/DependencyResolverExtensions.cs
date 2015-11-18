using FS.Cache;
using FS.DI.Resolve;
using FS.Extends;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.DI.Core
{
    public static class DependencyResolverExtensions
    {
        /// <summary>
        ///     解析依赖服务
        /// </summary>
        public static TService Resolve<TService>(this IDependencyResolver dependencyResolver)
            where TService : class
        {
            if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));
            return (TService)dependencyResolver.Resolve(typeof(TService));
        }

        /// <summary>
        ///     解析依赖服务集合
        /// </summary>
        public static IEnumerable<TService> ResolveAll<TService>(this IDependencyResolver dependencyResolver)
            where TService : class
        {
            if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));
            return dependencyResolver.ResolveAll(typeof(TService)).Select(t => (TService)t).Distinct(t => t.GetType());
        }

        /// <summary>
        ///     添加解析器调用
        /// </summary>
        public static void AddCallSite(this IDependencyResolver dependencyResolver, params IResolverCallSite[] callSites)
        {
            CallSiteCacheManager.SetCache(dependencyResolver, callSites);
        }

        /// <summary>
        ///     移除所有解析器调用
        /// </summary>
        /// <param name="dependencyResolver"></param>
        public static void RemoveAllCallSites(this IDependencyResolver dependencyResolver)
        {
            CallSiteCacheManager.Clear();
        }
    }
}
