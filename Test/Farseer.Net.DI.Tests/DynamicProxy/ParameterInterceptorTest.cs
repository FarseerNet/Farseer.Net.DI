using FS.DI.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class ParameterInterceptorTest
    {
        [TestMethod]
        public void ParameterInterceptor()
        {
            var proxy = (ParameterInterceptorTestClass)DynamicActivator.CreateInstance(typeof(ParameterInterceptorTestClass));

            /// Foo方法执行时对方法参数进行拦截
            proxy.Foo(null, 123);
        }
    }

    public class ParameterInterceptorTestClass
    {
        public virtual void Foo([ParameterValidity]string value, [ParameterValidity]int age)
        {
            Console.WriteLine(value);
            Console.WriteLine(age);
        }
    }

    public class ParameterValidityAttribute : ParameterInterceptorAttribute, IExceptionInterceptor, IMethodInterceptor
    {
        public int ExecutedOrder
        {
            get
            {
                return 1;
            }
        }

        public int ExecutingOrder
        {
            get
            {
                return 1;
            }
        }

        public void OnExcepion(IExceptionInvocation invocation)
        {
            Console.WriteLine(invocation.Exception);
        }

        public void OnMethodExecuted(IMethodInvocation invocation)
        {

        }

        public void OnMethodExecuting(IMethodInvocation invocation)
        {
            invocation.Parameters[0].Value = "leandro_";
        }

        public override void OnParameterExecuting(IParameterInvocation invocation)
        {
            foreach (var parameter in invocation.Parameters)
            {
                if (parameter.ParameterInfo.ParameterType.IsValueType)
                {
                    parameter.Value = 21;
                }
                else
                {
                    parameter.Value = "leandro";
                }
            }
        }
    }
}