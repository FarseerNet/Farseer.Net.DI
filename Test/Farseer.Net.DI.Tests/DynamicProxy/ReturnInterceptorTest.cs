using System;
using FS.DI.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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
        [return:ReturnValidity]
        public virtual object Foo()
        {
            return null;
        }
    }

    public class ReturnValidityAttribute : ReturnInterceptorAttribute
    {
        public override void OnReturnExecuted(IReturnInvocation invocation)
        {
            if (invocation.Parameter.Value == null)
                invocation.Parameter.Value = new ReturnInterceptorTestClass();
        }
    }
}
