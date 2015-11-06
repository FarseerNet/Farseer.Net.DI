namespace FS.DI.DynamicProxy
{
    public interface IExceptionInterceptor : IInterceptor
    {
        void OnExcepion(IExceptionInvocation invocation);
    }
}
