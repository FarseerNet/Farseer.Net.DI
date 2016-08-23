using FS.Cache;
using FS.Extends;
using System;
using System.Linq;
using System.Reflection;

namespace FS.DI.Resolve
{
    internal static class ResolverHelper
    {
        /// <summary>
        ///     是否解析完成
        /// </summary>
        internal static bool NotResolved(this IResolverContext context) => !context.Handled;

        /// <summary>
        ///     是否Transient生命周期
        /// </summary>
        internal static bool IsTransientLifetime(this IResolverContext context)
            => context.Dependency.Lifetime == DependencyLifetime.Transient;

        /// <summary>
        ///     是否Singleton生命周期
        /// </summary>
        internal static bool IsSingletonLifetime(this IResolverContext context)
            => context.Dependency.Lifetime == DependencyLifetime.Singleton;

        /// <summary>
        ///     是否Scoped生命周期
        /// </summary>
        internal static bool IsScopedLifetime(this IResolverContext context)
            => context.Dependency.Lifetime == DependencyLifetime.Scoped;

        /// <summary>
        ///     ImplementationType是否有值
        /// </summary>
        internal static bool HasImplementationType(this IResolverContext context)
            => context.Dependency.ImplementationType != null;

        /// <summary>
        ///     ImplementationInstance是否有值
        /// </summary>
        internal static bool HasImplementationInstance(this IResolverContext context)
            => context.Dependency.ImplementationInstance != null;

        /// <summary>
        ///     ImplementationDelegate是否有值
        /// </summary>
        internal static bool HasImplementationDelegate(this IResolverContext context)
            => context.Dependency.ImplementationDelegate != null;

        /// <summary>
        ///     是否含有公共的构造方法
        /// </summary>
        internal static bool HasPublicConstructor(this IResolverContext context)
            => context.Dependency.GetImplementationType()
                .GetConstructors()
                .Any(ctor => ctor.GetParameters().Length > 0);

        /// <summary>
        ///     是否生成动态代理类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsDynamicProxy(this IResolverContext context)
            => context.Dependency.Style.HasFlag(DependencyStyle.DynamicProxy);

        /// <summary>
        ///     返回最佳构造方法
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal static ConstructorInfo GetBastConstructor(Type type, IDependencyResolver resolver)
        {
            var constructors = type.GetConstructors().OrderBy(ctor => ctor.GetParameters().Length).ToArray();
            switch (constructors.Length)
            {
                case 0:
                    throw new InvalidOperationException(type.FullName + "类没有公共的构造方法。");
                case 1:
                    return constructors[0];
                default:
                    ConstructorInfo bestConstructor = null;
                    foreach (
                        var constructor in
                            constructors.Where(
                                constructor =>
                                    constructor.GetParameterTypes()
                                        .All(
                                            t =>
                                                DependencyCacheManager.GetCache((IScopedResolver) resolver, t) !=
                                                null)))
                    {
                        if (bestConstructor == null)
                        {
                            bestConstructor = constructor;
                        }
                        else
                        {
                            if (bestConstructor.GetParameters().Length == constructor.GetParameters().Length)
                            {
                                throw new InvalidOperationException("类型\"" + type.FullName + "\" 构造方法调用不明确。");
                            }
                            bestConstructor = constructor;
                        }
                    }
                    if (bestConstructor == null)
                    {
                        throw new InvalidOperationException("类型\"" + type.FullName + "\"未找到合适的构造方法。");
                    }
                    return bestConstructor;
            }
            throw new InvalidOperationException("类型\"" + type.FullName + "\"未找到合适的构造方法。");
        }

        internal static object[] GetConstructorParameters(Type type, IDependencyResolver resolver)
            =>
                GetBastConstructor(type, resolver)
                    .GetParameters()
                    .Select(p => resolver.Resolve(p.ParameterType))
                    .ToArray();
    }
}