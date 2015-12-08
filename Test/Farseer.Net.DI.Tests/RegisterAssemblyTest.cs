using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace FS.DI.Tests
{
    [TestClass]
    public class RegisterAssemblyTest
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        private readonly IFarseerContainer container = new FarseerContainer();

        [TestMethod]
        public void RegisterBaseType()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册实现特定接口或基类的类型
            var assembly = register.RegisterAssembly<IDependency>(Assembly.GetExecutingAssembly());
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                var results = resolver.ResolveAll<IDependency>();
                foreach (var value in results)
                {
                    Assert.IsNotNull(value);
                    Assert.IsInstanceOfType(value, typeof(IDependency));
                }
            }
        }

        [TestMethod]
        public void RegisterNamed()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册程序集中符合约定名称的类型
            register.RegisterAssembly("Repository", Assembly.GetExecutingAssembly());
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                var results = resolver.ResolveAll<IRepository<UserEntity>>();
                foreach (var value in results)
                {
                    Assert.IsNotNull(value);
                    Assert.IsInstanceOfType(value, typeof(IRepository<UserEntity>));
                }
            }
        }

        [TestMethod]
        public void RegisterGeneric()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册实现特定泛型类型定义IRepository<>的类型
            register.RegisterAssembly(typeof(IRepository<>), Assembly.GetExecutingAssembly());
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {

                ///解析实现特定泛型类型定义IRepository<>的类型
                var results = resolver.ResolveAll(typeof(IRepository<>));
                foreach (var value in results)
                {
                    Assert.IsNotNull(value);
                    Assert.IsInstanceOfType(value, typeof(IRepository<UserEntity>));
                }
            }
        }
    }
}
