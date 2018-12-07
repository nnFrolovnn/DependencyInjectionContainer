using System;
using DependencyInjectionContainer.Interfaces;
using DependencyInjectionContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIC_Test.TestClasses;

namespace DIC_Test
{
    [TestClass]
    public class DIContainerTests
    {
        IDIContainer creator;

        [ClassInitialize]
        public void ClassInit()
        {
            DIConfiguration config = new DIConfiguration();
            config.Register<IFoo, Foo>();

        }
        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}
