using FS.DI.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            proxy.Foo(null);
        }
    }

    public class ParameterInterceptorTestClass
    {
        public virtual void Foo([ParameterValidity]string value)
        {
        }
    }

    public class ParameterValidityAttribute : ParameterInterceptorAttribute
    {
        public override void OnParameterExecuting(IParameterInvocation invocation)
        {
            foreach (var parameter in invocation.Parameters)
            {
                if (parameter.ParameterInfo.ParameterType.IsValueType)
                {
                    if (parameter.ParameterInfo.ParameterType == typeof(int))
                        Debug.WriteLineIf((int)parameter.Value == 0, string.Format("参数{0}的值不能为0", parameter.Name));
                }
                else
                {
                    Debug.WriteLineIf(parameter.Value == null, string.Format("参数{0}的值不能为null", parameter.Name));
                }
            }
        }
    }
}