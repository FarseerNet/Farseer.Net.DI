using FS.DI.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class ExceptionInterceptorTest
    {
        [TestMethod]
        public void ExceptionInterceptor()
        {
            var proxy = (ExceptionInterceptorTestClass)DynamicActivator.CreateInstance(typeof(ExceptionInterceptorTestClass));

            /// Foo方法执行时异常拦截
            proxy.Foo();
        }
    }

    public class ExceptionInterceptorTestClass
    {
        [TestException]
        public virtual void Foo()
        {
            throw new Exception("出错啦！");
        }
    }

    public class TestExceptionAttribute : ExceptionInterceptorAttribute
    {
        public override void OnExcepion(IExceptionInvocation invocation)
        {
            ///处理异常
            Debug.WriteLine(invocation.Exception.Message);
        }
    }
}
