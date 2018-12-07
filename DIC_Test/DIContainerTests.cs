using System;
using DependencyInjectionContainer.Interfaces;
using DependencyInjectionContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIC_Test.TestClasses;
using System.Linq;

namespace DIC_Test
{
    [TestClass]
    public class DIContainerTests
    {
        [TestMethod]
        public void IsSingleton()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.RegisterSingleton<IBar, BarFromIBar>();

            DIContainer container = new DIContainer(conf);

            IBar bar1 = container.Resolve<IBar>();
            IBar bar2 = container.Resolve<IBar>();

            Assert.AreEqual(bar1, bar2);
        }

        [TestMethod]
        public void IsNotSingleton()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<IBar, BarFromABar>();

            DIContainer container = new DIContainer(conf);

            IBar bar1 = container.Resolve<BarFromABar>();
            IBar bar2 = container.Resolve<BarFromABar>();

            Assert.AreNotEqual(bar1, bar2);
        }

        [TestMethod]
        public void CorrectCreateDependency()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<IBar, BarFromIBar>();
            conf.Register<ABar, BarFromABar>();
            conf.Register<Foo, Foo>();

            DIContainer container = new DIContainer(conf);

            Foo foo = container.Resolve<Foo>();

            Assert.IsNotNull(foo.Bar);
        }

        [TestMethod]
        public void AbstractClassAdded()
        {
            DIConfiguration conf = new DIConfiguration();
            try
            {
                conf.Register<ABar, ABar>();
                Assert.Fail("registered class is abstract");
            }
            catch(Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void InsolvableCreation()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<IFoo, Foo>();
            conf.Register<IBar, BarFromIBar>();

            DIContainer container = new DIContainer(conf);

            try
            {
                IBar bar = container.Resolve<IBar>();
                Assert.Fail("Cannot be created");
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void NoSuchType()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<IFoo, Foo>();

            DIContainer container = new DIContainer(conf);

            try
            {
                IBar bar = container.Resolve<BarFromIBar>();
                Assert.Fail("Cannot create instance of not registered type");
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void RegisterClass()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<ABar, BarFromABar>();
            conf.Register<IFoo, Foo>();
            conf.Register<Foo>();

            DIContainer container = new DIContainer(conf);

            Foo foo = container.Resolve<Foo>();
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void RegInterfaceInterface()
        {
            try
            {
                var conf = new DIConfiguration();
                conf.Register<IBar>();

                var container = new DIContainer(conf);

                var bar = container.Resolve<IBar>();
                Assert.Fail("Cannot create instance of interface");
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void RegisterAbstractClass()
        {
            try
            {
                DIConfiguration conf = new DIConfiguration();
                conf.Register<ABar>();

                DIContainer container = new DIContainer(conf);

                ABar bar = container.Resolve<ABar>();
                Assert.Fail("Cannot create instance of abstract class");
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e, e.Message);
            }
        }

        [TestMethod]
        public void ResolveEnumerable()
        {
            var expected = 2;
            var conf = new DIConfiguration();
            conf.Register<IBar, BarFromIBar>();
            conf.Register<IBar, BarFromABar>();

            var container = new DIContainer(conf);

            var bars = container.ResolveAll<IBar>();
            Assert.AreEqual(expected, bars.Count());
        }

        [TestMethod]
        public void ResolveOpenGenType()
        {
            DIConfiguration conf = new DIConfiguration();
            conf.Register<IBar, BarFromABar>();
            conf.Register<IFoo, Foo>();
            conf.Register(typeof(IGenFoo<>), typeof(IGenFoo<>));

            DIContainer container = new DIContainer(conf);

            var genFoo = container.Resolve<IGenFoo<IFoo>>();
            Assert.IsNotNull(genFoo);
        }

        [TestMethod]
        public void ResolveGenType()
        {
            var conf = new DIConfiguration();
            conf.Register<IBar, BarFromABar>();
            conf.Register(typeof(IGenBar<IBar>), typeof(IGenBar<IBar>));

            var container = new DIContainer(conf);

            IGenBar<IBar> ogen = container.Resolve< IGenBar<IBar>>();
            Assert.IsNotNull(ogen);
        }
    }
}
