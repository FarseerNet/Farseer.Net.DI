using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests
{
    [TestClass]
    public class LifetimeTest
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        private readonly IFarseerContainer container = new FarseerContainer();

        [TestMethod]
        public void Transient()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册瞬时生命周期的类型
            register.RegisterType<IUserRepository, UserRepository>().AsTransientLifetime();
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository1 = resolver.Resolve<IUserRepository>();
                IUserRepository userRepository2 = resolver.Resolve<IUserRepository>();

                Assert.AreNotEqual<IUserRepository>(userRepository1, userRepository2);
                ///创建作用域
                using (IScopedResolver scoped = resolver.CreateScopedResolver())
                {
                    IUserRepository userRepository3 = scoped.Resolve<IUserRepository>();

                    Assert.AreNotEqual<IUserRepository>(userRepository1, userRepository3);
                    Assert.AreNotEqual<IUserRepository>(userRepository3, userRepository2);
                }
            }
        }

        [TestMethod]
        public void Singleton()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册瞬时生命周期的类型
            register.RegisterType<IUserRepository, UserRepository>().AsSingletonLifetime();
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository1 = resolver.Resolve<IUserRepository>();
                IUserRepository userRepository2 = resolver.Resolve<IUserRepository>();

                Assert.AreEqual<IUserRepository>(userRepository1, userRepository2);
                ///创建作用域
                using (IScopedResolver scoped = resolver.CreateScopedResolver())
                {
                    IUserRepository userRepository3 = scoped.Resolve<IUserRepository>();

                    Assert.AreEqual<IUserRepository>(userRepository1, userRepository3);
                    Assert.AreEqual<IUserRepository>(userRepository3, userRepository2);
                }
            }
        }

        [TestMethod]
        public void Scoped()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册瞬时生命周期的类型
            register.RegisterType<IUserRepository, UserRepository>().AsScopedLifetime();
            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                IUserRepository userRepository1 = resolver.Resolve<IUserRepository>();
                ///创建作用域
                using (IScopedResolver scoped = resolver.CreateScopedResolver())
                {
                    IUserRepository scopedRepository1 = scoped.Resolve<IUserRepository>();
                    IUserRepository scopedRepository2 = scoped.Resolve<IUserRepository>();

                    Assert.AreNotEqual<IUserRepository>(userRepository1, scopedRepository1);

                    Assert.AreEqual<IUserRepository>(scopedRepository1, scopedRepository2);
                    ///创建作用域
                    using (IScopedResolver scoped1 = scoped.CreateScopedResolver())
                    {
                        IUserRepository scopedRepository3 = scoped1.Resolve<IUserRepository>();
                        IUserRepository scopedRepository4 = scoped1.Resolve<IUserRepository>();

                        Assert.AreNotEqual<IUserRepository>(userRepository1, scopedRepository3);
                        Assert.AreNotEqual<IUserRepository>(scopedRepository1, scopedRepository3);

                        Assert.AreEqual<IUserRepository>(scopedRepository3, scopedRepository4);
                    }
                }
            }

            IFarseerContainer c1 = new FarseerContainer();
            ///创建注册器
            IDependencyRegister register1 = c1.CreateRegister();
            ///注册瞬时生命周期的类型
            register1.RegisterType<IUserRepository, UserRepository>().AsScopedLifetime();
            ///创建解析器
            using (IDependencyResolver resolver = c1.CreateResolver())
            {
                IUserRepository userRepository1 = resolver.Resolve<IUserRepository>();
                ///创建作用域
                using (IScopedResolver scoped = resolver.CreateScopedResolver())
                {
                    IUserRepository scopedRepository1 = scoped.Resolve<IUserRepository>();
                    IUserRepository scopedRepository2 = scoped.Resolve<IUserRepository>();

                    Assert.AreNotEqual<IUserRepository>(userRepository1, scopedRepository1);

                    Assert.AreEqual<IUserRepository>(scopedRepository1, scopedRepository2);
                    ///创建作用域
                    using (IScopedResolver scoped1 = scoped.CreateScopedResolver())
                    {
                        IUserRepository scopedRepository3 = scoped1.Resolve<IUserRepository>();
                        IUserRepository scopedRepository4 = scoped1.Resolve<IUserRepository>();

                        Assert.AreNotEqual<IUserRepository>(userRepository1, scopedRepository3);
                        Assert.AreNotEqual<IUserRepository>(scopedRepository1, scopedRepository3);

                        Assert.AreEqual<IUserRepository>(scopedRepository3, scopedRepository4);
                    }
                }
            }
        }
    }
}
