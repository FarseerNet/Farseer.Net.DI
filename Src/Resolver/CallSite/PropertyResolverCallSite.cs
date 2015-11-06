using FS.Cache;
using FS.DI.Core;
using FS.Extends;
using System;
using System.Linq;
using System.Reflection;
 
namespace FS.DI.Resolver.CallSite
{
    /// <summary>
    /// 属性注入解析器调用
    /// </summary>
    internal sealed class PropertyResolverCallSite : IResolverCallSite
    {
        private readonly IDependencyTable _dependencyTable;
        public PropertyResolverCallSite(IDependencyTable dependencyTable)
        {
            if (dependencyTable == null) throw new ArgumentNullException(nameof(dependencyTable));
            _dependencyTable = dependencyTable;
        }
        public bool PreResolver(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete();
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            var properties = context.CompleteValue.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).
                Where(property => _dependencyTable.PropertyEntryTable.ContainsKey(property.PropertyType));
            foreach (var property in properties)
            {
                try
                {
                    if (!property.IsDefined(typeof(IgnoreDependencyAttribute), false))
                    {
                        PropertySetCacheManger.Cache(property, context.CompleteValue, resolver.Resolve(property.PropertyType));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(String.Format("类型\"{0}\"未能注入属性\"{1}\"的实例。",
                        context.DependencyEntry.GetImplementationType(), property.PropertyType), ex);
                }
            }
            context.CompleteHandled = true;
        }
    }
}
