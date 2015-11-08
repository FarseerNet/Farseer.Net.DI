using FS.DI.Core;
using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests
{
    [TestClass]
    public class RegisterTest
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        private readonly IFarseerContainer container = new FarseerContainer();

        [TestMethod]
        public void RegisterType()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册类型
            register.RegisterType<IUserRepository, UserRepository>();
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository = resolver.Resolve<IUserRepository>();

                Assert.IsNotNull(userRepository);
                Assert.IsInstanceOfType(userRepository, typeof(IUserRepository));
            }            
        }

        [TestMethod]
        public void RegisterInstance()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册类型的实例
            UserRepository instance = new UserRepository();
            register.RegisterInstance(typeof(IUserRepository),instance);
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository = resolver.Resolve<IUserRepository>();

                Assert.IsNotNull(userRepository);
                Assert.IsInstanceOfType(userRepository, typeof(IUserRepository));
            }
        }

        [TestMethod]
        public void RegisterDelegate()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册委托           
            register.RegisterDelegate<IUserRepository, UserRepository>(resolver => new UserRepository());
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository = resolver.Resolve<IUserRepository>();

                Assert.IsNotNull(userRepository);
                Assert.IsInstanceOfType(userRepository, typeof(IUserRepository));
            }
        }

    }
}
