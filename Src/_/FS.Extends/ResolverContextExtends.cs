using FS.DI.Core;
using FS.DI.Resolve;
using System.Linq;

namespace FS.Extends
{
    internal static class ResolverContextExtends
    {
        /// <summary>
        /// 是否执行完成
        /// </summary>
        internal static bool NotComplete(this IResolverContext context)
        {
            return !context.Handled;
        }

        /// <summary>
        /// 是否Transient生命周期
        /// </summary>
        internal static bool IsTransientLifetime(this IResolverContext context)
        {
            return context.DependencyEntry.Lifetime == DependencyLifetime.Transient;
        }

        /// <summary>
        /// 是否Singleton生命周期
        /// </summary>
        internal static bool IsSingletonLifetime(this IResolverContext context)
        {
            return context.DependencyEntry.Lifetime == DependencyLifetime.Singleton;
        }

        /// <summary>
        /// 是否Scoped生命周期
        /// </summary>
        internal static bool IsScopedLifetime(this IResolverContext context)
        {
            return context.DependencyEntry.Lifetime == DependencyLifetime.Scoped;
        }

        /// <summary>
        /// ImplementationType是否有值
        /// </summary>
        internal static bool HasImplementationType(this IResolverContext context)
        {
            return context.DependencyEntry.ImplementationType != null;
        }

        /// <summary>
        /// ImplementationInstance是否有值
        /// </summary>
        internal static bool HasImplementationInstance(this IResolverContext context)
        {
            return context.DependencyEntry.ImplementationInstance != null;
        }

        /// <summary>
        /// ImplementationDelegate是否有值
        /// </summary>
        internal static bool HasImplementationDelegate(this IResolverContext context)
        {
            return context.DependencyEntry.ImplementationDelegate != null;
        }

        /// <summary>
        /// 是否含有公共的构造方法
        /// </summary>
        internal static bool HasPublicConstructor(this IResolverContext context)
        {
            return context.DependencyEntry.GetImplementationType().GetConstructors().Any(ctor => ctor.GetParameters().Length > 0);
        }
    }
}
