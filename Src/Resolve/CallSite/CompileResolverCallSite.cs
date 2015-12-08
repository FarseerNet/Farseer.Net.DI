using FS.Cache;
using FS.Extends;
using System;
using System.Diagnostics.CodeAnalysis;
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
                var args = context.HasImplementationType() && context.HasPublicConstructor()
                    ? ResolverHelper.GetConstructorParameters(
                        context.Dependency.GetImplementationType(), resolver)
                    : ArrayExtends.Empty<object>();

                context.Resolved = CompileCacheManager.GetOrSetCache(context.Dependency,
                    () => CreateDelegate(context.Resolved as Expression)).
                    Invoke(resolver, args);

                CacheResolved(context, resolver);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"未能创建类型\"{context.Dependency.ServiceType}\"的实例。", ex);
            }
        }

        /// <summary>
        ///     编译表达式树生成委托
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private Func<IDependencyResolver, object[], object> CreateDelegate(Expression body)
            => (body as Expression<Func<IDependencyResolver, object[], object>>).Compile();

        private void CacheResolved(IResolverContext context, IDependencyResolver resolver)
        {
            if (context.IsSingletonLifetime())
            {
                ScopedCacheManager.SetCache(((DependencyResolver) resolver).RootScoped, context.Dependency,
                    context.Resolved);
            }
            if (context.IsScopedLifetime())
            {
                ScopedCacheManager.SetCache((IScopedResolver) resolver, context.Dependency, context.Resolved);
            }
        }
    }
}
