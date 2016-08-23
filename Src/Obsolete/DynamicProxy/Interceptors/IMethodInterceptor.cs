namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     方法拦截器
    /// </summary>
    public interface IMethodInterceptor : IInterceptor
    {
        int ExecutingOrder { get; }

        int ExecutedOrder { get; }

        /// <summary>
        ///     方法执行前拦截
        /// </summary>
        void OnMethodExecuting(IMethodInvocation invocation);

        /// <summary>
        ///     方法执行后拦截
        /// </summary>
        void OnMethodExecuted(IMethodInvocation invocation);      
    }
}
