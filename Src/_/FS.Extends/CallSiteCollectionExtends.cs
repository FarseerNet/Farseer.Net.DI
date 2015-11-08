using FS.DI.Resolve;
using FS.DI.Resolve.CallSite;

namespace FS.Extends
{
    static class CallSiteCollectionExtends
    {
        /// <summary>
        /// 添加默认解析器实现
        /// </summary>
        internal static void AddDefault(this ICallSiteCollection collection, IDependencyTable dependencyTable)
        {
            collection.Add(new PropertyResolverCallSite(dependencyTable));
            collection.Add(new CompileResolverCallSite(dependencyTable));
            collection.Add(new ConstructorResolverCallSite(dependencyTable));
            collection.Add(new NonConstructorResolverCallSite());
            collection.Add(new InstanceResolverCallSite());
            collection.Add(new DelegateResolverCallSite());
            collection.Add(new ScopedResolverCallSite(dependencyTable));
            collection.Add(new SingletonResolverCallSite(dependencyTable));
            collection.Add(new TransientResolverCallSite(dependencyTable));
        }
    }
}
