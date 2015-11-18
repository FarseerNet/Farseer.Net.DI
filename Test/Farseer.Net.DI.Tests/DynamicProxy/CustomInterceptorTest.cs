using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FS.DI.DynamicProxy;
using FS.DI.Tests.Infrastructure;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class CustomInterceptorTest
    {
        [TestMethod]
        public void ConfigureCustomInterceptor()
        {
            CustomInterceptorManager.Configure(typeof(CustomInterceptorTestClass), new CustomInterceptor());
            var proxy = (CustomInterceptorTestClass)DynamicActivator.CreateInstance(typeof(CustomInterceptorTestClass));
            proxy.Foo();
        }


       
    }

    public class CustomInterceptorTestClass
    {
        public virtual void Foo()
        {
        }
    }
}
