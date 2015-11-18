using FS.DI.Core;
using FS.DI.Resolve;
using FS.DI.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace FS.DI.Tests
{
    [TestClass]
    public class ResolverCallSiteTest
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        private readonly IFarseerContainer container = new FarseerContainer();

        [TestMethod]
        public void ResolverCallSiteExtensions()
        {
            ///创建注册器
            IDependencyRegister register = container.CreateRegister();
            ///注册类型
            register.RegisterType<IRepository<UserEntity>, UserRepository>();
            register.RegisterType<IUserService, UserService>();

            ///创建解析器
            using (IDependencyResolver resolver = container.CreateResolver())
            {
                ///清除默认解析器
                resolver.RemoveAllCallSites();
                ///添加自定义解析器
                resolver.AddCallSite(new CustomResolverCallSite());

                IUserService service = resolver.Resolve<IUserService>();
                Assert.IsNotNull(service);
            }
        }
    }

    /// <summary>
    /// 自定义解析器
    /// </summary>
    public class CustomResolverCallSite : IResolverCallSite
    {
        /// <summary>
        /// 解析前置条件 / 返回ture则执行Resolver方法
        /// </summary>
        public bool Requires(IResolverContext context, IDependencyResolver resolver)
        {
            return context.DependencyEntry.GetImplementationType().GetConstructors().Any();
        }
        /// <summary>
        /// 解析类型
        /// </summary>
        public void Resolver(IResolverContext context, IDependencyResolver resolver)
        {
            ConstructorInfo constructor = FindConstructor(context);
            Object[] args = GetParameterValues(constructor, resolver);

            ///设置解析结果
            context.Resolved = constructor.Invoke(args);
            ///解析完成
            context.Handled = true;
        }

        private ConstructorInfo FindConstructor(IResolverContext context)
        {
            ///返回实现类型参数最多的构造器
            return context.DependencyEntry.GetImplementationType().  ///获取服务的实现类型
                GetConstructors().
                OrderByDescending(
                ctor => ctor.GetParameters().Length).
                First();
        }

        private Object[] GetParameterValues(ConstructorInfo constructor, IDependencyResolver resolver)
        {
            ///解析构造器参数类型
            return constructor.GetParameters().
                Select(
                parameter => resolver.Resolve(
                    parameter.ParameterType)).
                ToArray();
        }
    }
}
