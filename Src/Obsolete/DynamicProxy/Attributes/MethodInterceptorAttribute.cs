using System;

namespace FS.DI.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public abstract class MethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public virtual int ExecutedOrder { get; set; }

        public virtual int ExecutingOrder { get; set; }

        public abstract void OnMethodExecuted(IMethodInvocation invocation);
        
        /// <summary>
        /// 方法执行前拦截
        /// </summary>
        public abstract void OnMethodExecuting(IMethodInvocation invocation);
         
    }
}
