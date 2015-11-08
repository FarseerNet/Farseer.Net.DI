using FS.DI.Core;
using FS.Extends;
using System;
using System.Linq.Expressions;

namespace FS.DI.Resolve.CallSite
{
    /// <summary>
    /// 编译解析器
    /// </summary>
    internal sealed class CompileResolverCallSite : IResolverCallSite
    {
        private readonly IDependencyTable _dependencyTable;
        public CompileResolverCallSite(IDependencyTable dependencyTable)
        {
            if (dependencyTable == null) throw new ArgumentNullException(nameof(dependencyTable));
            _dependencyTable = dependencyTable;
        }
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.NotComplete() && context.Value is Expression;
        }

        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            try
            {
                var factory = _dependencyTable.GetOrAddCompile(context.DependencyEntry,
                    (serviceType, iImplementationType) => (CreateDelegate(context.Value as Expression)));

                Object[] args = GetParameters(context, _dependencyTable, resolver);
     
                var completeValue = factory.Invoke(resolver, args);
                context.Value = completeValue;
                CacheComplete(context, resolver);
                context.Handled = !_dependencyTable.HasPropertyEntryTable.ContainsKey(context.DependencyEntry);        
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("未能创建类型\"{0}\"的实例。", context.DependencyEntry.ServiceType), ex);
            }
        }

        private Object[] GetParameters(IResolverContext context, IDependencyTable dependencyTable, IDependencyResolver resolver)
        {
            return context.HasImplementationDelegate() ?
                    new Object[0] : context.HasPublicConstructor() ? 
                    context.DependencyEntry.GetImplementationType().
                    GetConstructorParameters(dependencyTable, resolver) : 
                    new Object[0];
        }
        /// <summary>
        /// 编译表达式树生成委托
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Func<IDependencyResolver, Object[], Object> CreateDelegate(Expression body)
        {          
            return (body as Expression<Func<IDependencyResolver, Object[], Object>>).Compile();
        }

        private void CacheComplete(IResolverContext context, IDependencyResolver resolver)
        {
            if (context.IsSingletonLifetime())
            {
                _dependencyTable.AddScoped(context, null);
            }
            if (context.IsScopedLifetime())
            {
                _dependencyTable.AddScoped(context, resolver);
            }
        }
    }
}
