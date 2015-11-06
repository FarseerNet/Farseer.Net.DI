namespace FS.DI.DynamicProxy
{
    public interface IParameterInterceptor : IInterceptor
    {
        void OnParameterExecuting(IParameterInvocation invocation);
    }
}
