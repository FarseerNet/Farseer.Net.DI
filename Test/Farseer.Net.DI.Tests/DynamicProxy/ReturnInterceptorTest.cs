using System;
using FS.DI.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class ReturnInterceptorTest
    {
        [TestMethod]
        public void ReturnInterceptor()
        {
            var proxy = (ReturnInterceptorTestClass)DynamicActivator.CreateInstance(typeof(ReturnInterceptorTestClass));

            /// Foo方法执行时对方法返回值进行拦截
            var result = proxy.Foo();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ReturnInterceptorTestClass));
        }
    }

    public class ReturnInterceptorTestClass
    {
        [ReturnValidity]
        public virtual object Foo()
        {
            return null;
        }
    }

    public class ReturnValidityAttribute : MethodInterceptorAttribute
    {
        public override void OnMethodExecuted(IMethodInvocation invocation)
        {
            if (invocation.ReturnParameter.Value == null)
                invocation.ReturnParameter.Value = new ReturnInterceptorTestClass();
        }

        public override void OnMethodExecuting(IMethodInvocation invocation)
        {     
        }
    }
}
