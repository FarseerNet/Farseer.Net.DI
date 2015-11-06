using System;

namespace FS.DI.DynamicProxy
{
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    public abstract class ReturnInterceptorAttribute : Attribute, IReturnInterceptor
    {
        public abstract void OnReturnExecuted(IReturnInvocation invocation);
    }
}