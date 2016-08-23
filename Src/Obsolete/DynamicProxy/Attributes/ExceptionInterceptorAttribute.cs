using System;

namespace FS.DI.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public abstract class ExceptionInterceptorAttribute : Attribute, IExceptionInterceptor
    {     
        public virtual void OnExcepion(IExceptionInvocation invocation)
        {
            throw invocation.Exception;
        }
    }
}