using FS.Cache;
using FS.DI.Core;
using FS.DI.Resolve.CallSite;
using FS.Extends;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.DI.Resolve
{
    /// <summary>
    /// 默认解析器实现
    /// </summary>
    internal sealed class DependencyResolver : IDependencyResolver, IScopedResolver
    {
        internal IScopedResolver RootScoped { get; private set; }

        public DependencyResolver(IEnumerable<DependencyEntry> dependencyEntries)
        {
            RootScoped = this;
            DependencyEntryCacheManager.SetCache(this, dependencyEntries);
            InitializationCallSite();
        }

        public DependencyResolver(IScopedResolver root, IEnumerable<DependencyEntry> dependencyEntries)
        {
            RootScoped = root;
            DependencyEntryCacheManager.SetCache(this, dependencyEntries);
            InitializationCallSite();
        }

        private void InitializationCallSite()
        {
            CallSiteCacheManager.SetCache(this, new IResolverCallSite[] {
                new PropertyResolverCallSite(),
                new CompileResolverCallSite(),
                new ConstructorResolverCallSite(),
                new NewResolverCallSite(),
                new InstanceResolverCallSite(),
                new DelegateResolverCallSite(),
                new ScopedResolverCallSite(),
                new SingletonResolverCallSite(),
                new TransientResolverCallSite()
            });
        }

        public void Dispose()
        {
            ScopedKeyCacheManager.RemoveCache(this);
            DependencyEntryCacheManager.RemoveCache(this);
            CallSiteCacheManager.RemoveCache(this);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        public Object Resolve(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            DependencyEntry entry = DependencyEntryCacheManager.GetCache(this, serviceType);
            if (entry == null)
            {
                throw new InvalidOperationException(string.Format("尝试解析未注册的类型\"{0}\"失败。", serviceType.FullName));
            }
            return BuildUp(new ResolverContext(entry.Last));
        }

        Object IServiceProvider.GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }

        /// <summary>
        ///     创建作用域解析器
        /// </summary>
        /// <returns></returns>
        public IScopedResolver CreateScopedResolver()
        {
            return new DependencyResolver(RootScoped, DependencyEntryCacheManager.GetCache(this));
        }

        /// <summary>
        ///     解析服务集合
        /// </summary>
        public IEnumerable<Object> ResolveAll(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (serviceType.IsGenericTypeDefinition)
            {
                var serviceTypes = DependencyEntryCacheManager.GetCache(this).
                    Where(e => e.ServiceType.GetGenericTypeDefinitions().
                    Any(genericType => genericType == serviceType)).
                    Select(e => e.ServiceType).ToArray();
                foreach (var type in serviceTypes)
                {
                    DependencyEntry entry = DependencyEntryCacheManager.GetCache(this, type);
                    if (entry == null)
                    {
                        throw new InvalidOperationException(string.Format("尝试解析未注册的类型\"{0}\"失败。", serviceType.FullName));
                    }
                    foreach (var e in entry.Chain())
                    {
                        yield return BuildUp(new ResolverContext(e));
                    }
                }
            }
            else
            {
                DependencyEntry entry = DependencyEntryCacheManager.GetCache(this, serviceType);
                if (entry == null)
                {
                    throw new InvalidOperationException(string.Format("尝试解析未注册的类型\"{0}\"失败。", serviceType.FullName));
                }
                foreach (var e in entry.Chain())
                {
                    yield return BuildUp(new ResolverContext(e));
                }
            }
        }


        private Object BuildUp(IResolverContext context)
        {
            var callSiteCollection = CallSiteCacheManager.GetCache(this).ToArray();
            for (int i = callSiteCollection.Length - 1; i >= 0; i--)
            {
                var callSite = callSiteCollection[i];

                if (!callSite.Requires(context, this))
                    continue;

                callSite.Resolver(context, this);

                if (!context.Handled)
                    continue;

                return context.Resolved;
            }
            return context.Resolved;
        }
    }
}