using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FS.DI.Tests
{
    [TestClass]
    public class DisposableTest
    {
        [TestMethod]
        public void Disposable()
        {
            IFarseerContainer container = new FarseerContainer();
            var register = container.CreateRegister();
            register.RegisterType<DisposableTestClass>().AsScopedLifetime();
            using (var resolver = container.CreateResolver())
            {
                using (var scoped = resolver.CreateScopedResolver())
                {
                    var dis = resolver.Resolve<DisposableTestClass>();
                    var dis2 = scoped.Resolve<DisposableTestClass>();
                }
            }
        }
    }

    public class DisposableTestClass : IDisposable
    {
        private bool isDispose = false;
        public void Dispose()
        {
            if (isDispose)
                throw new Exception("Already  Disposed！");
            isDispose = true;
        }
    }
}
