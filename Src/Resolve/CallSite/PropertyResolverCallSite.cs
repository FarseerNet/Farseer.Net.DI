using FS.Cache;
using FS.DI.Core;
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
            var properties = PropertyCacheManager.GetOrSetCache(context.DependencyEntry, () =>
                context.Resolved.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).
                Where(property => DependencyEntryCacheManager.GetCache((IScopedResolver)resolver).
                Any(d => property.PropertyType == d.ServiceType) &&
                !property.IsDefined(typeof(IgnoreDependencyAttribute), false)));
            foreach (var property in properties)
            {
                try
                {
                    PropertySetCacheManger.Cache(property, context.Resolved, resolver.Resolve(property.PropertyType));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(String.Format("类型\"{0}\"未能注入属性\"{1}\"的实例。",
                        context.DependencyEntry.GetImplementationType(), property.PropertyType), ex);
                }
            }
            context.Handled = true;
        }
    }
}
