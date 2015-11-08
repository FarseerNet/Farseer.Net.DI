using FS.DI.Core;
using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests
{
    [TestClass]
    public class ConstructorInjectionTest
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        private readonly IFarseerContainer container = new FarseerContainer();

        [TestMethod]
        public void Constructor()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册类型
            register.RegisterType<IRepository<UserEntity>, UserRepository>();
            register.RegisterType<IUserService, UserService>();

            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserService service = resolver.Resolve<IUserService>();
                UserEntity entity = service.GetById(1);

                Assert.IsNotNull(entity);
                Assert.AreEqual(entity.Id, 1);
            }
        }
    }
}
