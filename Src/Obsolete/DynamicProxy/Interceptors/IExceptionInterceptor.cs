namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     异常拦截器
    /// </summary>
    public interface IExceptionInterceptor : IInterceptor
    {
        /// <summary>
        ///     异常拦截
        /// </summary>
        void OnExcepion(IExceptionInvocation invocation);
    }
}
