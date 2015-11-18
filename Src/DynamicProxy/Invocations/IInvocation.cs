using System;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     代理调用
    /// </summary>
    public interface IInvocation
    {
        /// <summary>
        ///     被拦截的类型
        /// </summary>
        Type InterceptedType { get; }

        /// <summary>
        ///      被拦截的实例
        /// </summary>
        Object InterceptedInstance { get; }
    }
}
