using FS.Cache;
using System;
using System.Linq;
using System.Reflection;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 属性注入解析器调用
    /// </summary>
    internal sealed class PropertyResolverCallSite : IResolverCallSite
    {
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotResolved();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            var properties = PropertyCacheManager.GetOrSetCache(context.Dependency,
                () =>
                    context.Resolved.GetType().
                        GetProperties(BindingFlags.Public | BindingFlags.Instance).
                        Where(property => DependencyCacheManager.GetCache((IScopedResolver) resolver).
                            Any(dependency => property.PropertyType == dependency.ServiceType) &&
                                          !property.IsDefined(typeof (IgnoreDependencyAttribute), false)));

            foreach (var property in properties)
            {
                try
                {
                    PropertySetCacheManger.Cache(property, context.Resolved, resolver.Resolve(property.PropertyType));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"类型\"{context.Dependency.GetImplementationType()}\"未能注入属性\"{property.PropertyType}\"的实例。",
                        ex);
                }
            }
            context.Handled = true;
        }
    }
}
