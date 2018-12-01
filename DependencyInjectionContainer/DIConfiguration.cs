using DependencyInjectionContainer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    class DIConfiguration : IDIConfiguration
    {
        private readonly ConcurrentDictionary<Type, object> dictionary;

        public DIConfiguration()
        {
            dictionary = new ConcurrentDictionary<Type, object>();
        }

        public IEnumerable<object> GetRegisteredTypes(Type type)
        {
            throw new NotImplementedException();
        }

        public void Regiser<TImplementation>() where TImplementation : class
        {
            Register(typeof(TImplementation), typeof(TImplementation));
        }

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type tInterface, Type tImplementation)
        {
            throw new NotImplementedException();
        }
    }
}
