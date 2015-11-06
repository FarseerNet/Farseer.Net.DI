namespace FS.DI.DynamicProxy
{
    public interface IReturnInterceptor : IInterceptor
    {
        void OnReturnExecuted(IReturnInvocation invocation);
    }
}
