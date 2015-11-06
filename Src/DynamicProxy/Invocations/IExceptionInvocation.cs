using System;

namespace FS.DI.DynamicProxy
{
    public interface IExceptionInvocation : IInvocation
    {
        /// <summary>
        ///     获取异常对象。
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        ///     获取或设置一个值，该值指示是否已处理异常。
        ///     值为true时将不执行后续异常拦截器。
        /// </summary>
        bool ExceptionHandled { get; set; }
    }
}
