using FS.Cache;
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
        internal IScopedResolver RootScoped { get; }

        public DependencyResolver(IEnumerable<Dependency> dependencyEntries)
        {
            RootScoped = this;
            DependencyCacheManager.SetCache(this, dependencyEntries);
            InitializationCallSite();
        }

        private DependencyResolver(IScopedResolver root, IEnumerable<Dependency> dependencyEntries)
        {
            RootScoped = root;
            DependencyCacheManager.SetCache(this, dependencyEntries);
            InitializationCallSite();
        }

        private void InitializationCallSite()
        {
            CallSiteCacheManager.SetCache(this,
                new PropertyResolverCallSite(),
                new CompileResolverCallSite(),
                new ConstructorResolverCallSite(),
                new NewResolverCallSite(),
                new InstanceResolverCallSite(),
                new DelegateResolverCallSite(),
                new ScopedResolverCallSite(),
                new SingletonResolverCallSite(),
                new TransientResolverCallSite());
        }

        public void Dispose()
        {
            ScopedKeyCacheManager.RemoveCache(this);
            DependencyCacheManager.RemoveCache(this);
            CallSiteCacheManager.RemoveCache(this);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public object Resolve(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            var dependency = DependencyCacheManager.GetCache(this, serviceType);
            if (dependency == null)
            {
                throw new InvalidOperationException($"尝试解析未注册的类型\"{serviceType.FullName}\"失败。");
            }
            return BuildUp(new ResolverContext(dependency.Last));
        }

        object IServiceProvider.GetService(Type serviceType) => Resolve(serviceType);

        /// <summary>
        ///     创建作用域解析器
        /// </summary>
        /// <returns></returns>
        public IScopedResolver CreateScopedResolver()
            => new DependencyResolver(RootScoped, DependencyCacheManager.GetCache(this));

        /// <summary>
        ///     解析服务集合
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (serviceType.IsGenericTypeDefinition)
            {
                var serviceTypes = DependencyCacheManager.GetCache(this).
                    Where(e => e.ServiceType.GetGenericTypeDefinitions().
                        Any(genericType => genericType == serviceType)).
                    Select(e => e.ServiceType).ToArray();
                foreach (var dependency in serviceTypes.Select(type => DependencyCacheManager.GetCache(this, type)))
                {
                    if (dependency == null)
                    {
                        throw new InvalidOperationException($"尝试解析未注册的类型\"{serviceType.FullName}\"失败。");
                    }
                    foreach (var e in dependency.Chain())
                    {
                        yield return BuildUp(new ResolverContext(e));
                    }
                }
            }
            else
            {
                var dependency = DependencyCacheManager.GetCache(this, serviceType);
                if (dependency == null)
                {
                    throw new InvalidOperationException($"尝试解析未注册的类型\"{serviceType.FullName}\"失败。");
                }
                foreach (var e in dependency.Chain())
                {
                    yield return BuildUp(new ResolverContext(e));
                }
            }
        }


        private object BuildUp(IResolverContext context)
        {
            var callSiteCollection = CallSiteCacheManager.GetCache(this).ToArray();
            var i = callSiteCollection.Length - 1;
            for (; i >= 0; i--)
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