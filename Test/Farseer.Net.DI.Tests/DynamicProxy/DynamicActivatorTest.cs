using FS.DI.DynamicProxy;
using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class DynamicActivatorTest
    {
        [TestMethod]
        public void CreateInstanceOfDefault()
        {
            ///使用指定类型的默认构造函数来创建该类型的代理实例。
            var proxy = DynamicActivator.CreateInstance(typeof(UserRepository));
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(proxy, typeof(UserRepository));
        }

        [TestMethod]
        public void CreateInstanceOfArgs()
        {
            ///使用指定类型的默认构造函数来创建该类型的代理实例。
            var proxyRepository = DynamicActivator.CreateInstance(typeof(UserRepository));

            /// 使用与指定参数匹配程度最高的构造函数创建指定类型的代理实例。
            var proxyService = DynamicActivator.CreateInstance(typeof(UserService), new[] { proxyRepository });
            Assert.IsNotNull(proxyService);
            Assert.IsInstanceOfType(proxyService, typeof(UserService));
        }
    }
    public class CreateInstanceTestClass
    {
        public virtual void Foo(string value)
        {

        }
    }
}
