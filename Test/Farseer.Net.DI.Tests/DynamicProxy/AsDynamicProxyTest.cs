using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class AsDynamicProxyTest
    {
        [TestMethod]
        public void AsDynamicProxy()
        {
            IFarseerContainer container = new FarseerContainer();
            var register = container.CreateRegister();

            ///注册为动态代理
            register.RegisterType<UserRepository>().AsDynamicProxy();

            using (var resolver = container.CreateResolver())
            {
                var proxy = resolver.Resolve<UserRepository>();
                proxy.GetById(1);
                Assert.IsNotNull(proxy);
            }
        }
    }
}
