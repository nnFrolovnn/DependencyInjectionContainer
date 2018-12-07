using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIC_Test.TestClasses
{
    internal interface IFoo { }

    internal abstract class AFoo : IFoo { }

    internal interface IBar { }

    internal abstract class ABar :IBar {}

    internal class BarFromABar : ABar
    {

    }

    internal class BarFromIBar : IBar
    {
        AFoo foo;

        public BarFromIBar(AFoo foo)
        {
            this.foo = foo;
        }
    }

    internal class Foo : IFoo
    {
        public IBar Bar { get; }

        public Foo(IBar bar)
        {
            Bar = bar;
        }
    }

    internal class IEnumFoo<T>
    {
        T val;
    }

    internal class IEnumBar<T> where T : class
    {
        public IEnumBar(T val)
        {

        }
    }

}
