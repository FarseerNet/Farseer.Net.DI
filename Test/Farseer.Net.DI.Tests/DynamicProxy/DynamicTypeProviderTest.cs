using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.DI.Tests.DynamicProxy
{
    [TestClass]
    public class DynamicTypeProviderTest
    {
        [TestMethod]
        public void CreateType()
        {

            ///获取当前的动态类型创建器
            //IDynamicTypeProvider typeProvider = DynamicTypeProvider.Current;

            /////根据指定类型创建动态类型(未缓存,多次创建同一动态类型会异常,建议使用DynamicActivator直接创建实例)
            //Type dynamicType = typeProvider.CreateType(typeof(UserRepository));

            /////创建动态类型的实例 
            //UserRepository proxyReposition = (UserRepository)Activator.CreateInstance(dynamicType);

            //Assert.IsNotNull(proxyReposition);
            //Assert.IsInstanceOfType(proxyReposition, typeof(UserRepository));
        }
    }
}
