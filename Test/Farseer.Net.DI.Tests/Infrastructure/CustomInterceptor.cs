using FS.DI.DynamicProxy;
using System.Diagnostics;

namespace FS.DI.Tests.Infrastructure
{
    public class CustomInterceptor : ICustomInterceptor, IMethodInterceptor
    {
        public int ExecutedOrder { get; } = 0;

        public int ExecutingOrder { get; } = 0;

        public void OnMethodExecuted(IMethodInvocation invocation)
        {
            Debug.WriteLine("CustomInterceptor Executed");
        }

        public void OnMethodExecuting(IMethodInvocation invocation)
        {
            Debug.WriteLine("CustomInterceptor Executing");
        }
    }
}
