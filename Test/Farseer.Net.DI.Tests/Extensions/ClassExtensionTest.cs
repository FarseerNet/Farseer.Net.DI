using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FS.DI.DynamicProxy.Expressions;
using FS.DI.Tests.Infrastructure;

namespace FS.DI.Tests.Expressions
{
    [TestClass]
    public class ClassExtensionTest
    {
        [TestMethod]
        public void Class()
        {
            var @class = Extension.Class(typeof(UserRepository));

        }
    }
}
