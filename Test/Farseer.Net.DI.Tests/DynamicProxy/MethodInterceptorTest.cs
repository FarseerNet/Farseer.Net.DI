using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using FS.DI.DynamicProxy;
using System.Diagnostics;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class MethodInterceptorTest
    {
        [TestMethod]
        public void MethodInterceptor()
        {
            var proxy = (MethodInterceptorTestClass)DynamicActivator.CreateInstance(typeof(MethodInterceptorTestClass));

            /// Foo方法执行拦截
            proxy.Foo();
        }
    }

    public class MethodInterceptorTestClass
    {
        [MethodTimer]
        public virtual void Foo()
        {
            Thread.Sleep(500);
        }
    }

    public class MethodTimerAttribute : MethodInterceptorAttribute
    {
        private  Stopwatch _stopwatch;
        public override void OnMethodExecuted(IMethodInvocation invocation)
        {
            ///方法执行后执行
            _stopwatch.Stop();
            Debug.WriteLine("方法执行时间：" + _stopwatch.Elapsed + "s");
        }

        public override void OnMethodExecuting(IMethodInvocation invocation)
        {
            ///方法执行前开始计时器
            _stopwatch = Stopwatch.StartNew();
        }
    }

}
