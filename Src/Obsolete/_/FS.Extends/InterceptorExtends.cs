using FS.Cache;
using FS.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     一个代码很乱的类，请忽略。
    /// </summary>
    public static class InterceptorExtends
    {
        /// <summary>
        /// 需要忽略的方法列表
        /// </summary>
        private const string SkipLoadingPattern = @"^Finalize|^GetHashCode|^Equal|^ToString";

        /// <summary>
        ///     搜索方法的所有拦截器
        /// </summary>
        public static IInterceptor[] GetInterceptors(this MethodInfo method, Type parentType)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (parentType == null) throw new ArgumentNullException(nameof(parentType));
            ///获取方法拦截器
            var interceptors = method.GetCustomAttributes(false).ToInterceptor();
            ///获取类型拦截器
            interceptors = interceptors.Concat(parentType.GetCustomAttributes(false).ToInterceptor());
            ///获取参数拦截器
            interceptors = interceptors.Concat(method.GetParameters().
                SelectMany(p => p.GetCustomAttributes(false), (p, i) => i).ToInterceptor());
            ///获取返回值拦截器
            interceptors =
                interceptors.Concat(method.ReturnTypeCustomAttributes.GetCustomAttributes(false).ToInterceptor());
            /// 获取自定义拦截器
            interceptors = interceptors.Concat(CustomInterceptorCacheManager.GetCache(parentType));
            return interceptors.ToArray();
        }

        /// <summary>
        ///     搜索可被代理类重写的方法
        /// </summary>
        public static MethodInfo[] GetProxyMethods(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.NonPublic).
                Where(method => method.CanProxy()).
                ToArray();
        }

        /// <summary>
        ///     指示方法能否被代理类重写
        /// </summary>
        public static bool CanProxy(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return !method.IsPrivate && !method.IsFinal && method.IsVirtual && !method.IsAssembly &&
                   !Regex.IsMatch(method.Name, SkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private static IEnumerable<IInterceptor> ToInterceptor<TSource>(this IEnumerable<TSource> source)
            => source.OfType<IInterceptor>();

        public static IExceptionInterceptor[] GetExcepionInterceptors(this IEnumerable<IInterceptor> interceptors)
            => interceptors.OfType<IExceptionInterceptor>().ToArray();

    
        public static IParameterInterceptor[] GetParameterInterceptors(this IEnumerable<IInterceptor> interceptors)
            => interceptors.OfType<IParameterInterceptor>().Distinct(i => i.GetType()).ToArray();

        public static IMethodInterceptor[] GetMethodInterceptor(this IEnumerable<IInterceptor> interceptors)
            => interceptors.OfType<IMethodInterceptor>().OrderBy(i => i.ExecutingOrder).ToArray();

        public static IMethodInterceptor[] GetExecutedMethodInterceptor(this IEnumerable<IInterceptor> interceptors)
            => interceptors.OfType<IMethodInterceptor>().OrderBy(i => i.ExecutedOrder).ToArray();

        public static MethodInfo GetInterceptedMethod()
        {
            var proxyMethod = (MethodInfo) new StackFrame(1).GetMethod();
            return proxyMethod.DeclaringType?.BaseType?.GetMethods().First(m => m.EqualMethod(proxyMethod));
        }

        public static MethodInfo GetIntercepted_IMethod(string @interface)
        {
            var proxyMethod = (MethodInfo) new StackFrame(1).GetMethod();
            var interfaceType = proxyMethod?.DeclaringType?.GetInterface(@interface);
            return interfaceType?.GetMethods().First(m => m.EqualMethod(proxyMethod));
        }

        public static MethodInfo MakeGenericMethod(MethodInfo method)
            => method.MakeGenericMethod(method.GetGenericArguments().Select(s => s).ToArray());

        public static ParameterInfo GetCurrentParameter(MethodInfo method, int index) => method.GetParameters()[index];

        public static bool EqualMethod(this MethodInfo method1, MethodInfo method2)
        {
            if (method1.Name.Split('.').Last() != method2.Name.Split('.').Last())
                return false;
            if (method1.IsGenericMethod && !method2.IsGenericMethod)
                return false;
            if (!method1.IsGenericMethod && method2.IsGenericMethod)
                return false;
            var parameters1 = method1.GetParameters();
            var parameters2 = method2.GetParameters();
            if (parameters1.Length != parameters2.Length)
                return false;
            return !parameters1.Where((t, i) => t.ParameterType.Name != parameters2[i].ParameterType.Name).Any();
        }

    }
}
