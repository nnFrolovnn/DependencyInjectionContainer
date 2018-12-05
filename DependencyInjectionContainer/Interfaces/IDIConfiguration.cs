using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.Interfaces
{
    public interface IDIConfiguration
    {
        void Register<TInterface, TImplementation>();

        void Register(Type tInterface, Type tImplementation);

        void Register<TImplementation>() where TImplementation : class;

        IEnumerable<ConfiguratedType> GetConfiguratedTypes(Type type);

        ConfiguratedType GetConfiguratedType(Type tinterface);
    }
}
