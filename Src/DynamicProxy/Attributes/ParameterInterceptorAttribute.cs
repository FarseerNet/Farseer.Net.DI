using System;

namespace FS.DI.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public abstract class ParameterInterceptorAttribute : Attribute, IParameterInterceptor
    {
        public abstract void OnParameterExecuting(IParameterInvocation invocation);
    }
}
