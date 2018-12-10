using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer.Interfaces
{
    public interface IDIConfiguration
    {
        IDictionary<Type, List<ConfiguratedType>> RegisteredTypesDictionary { get; }

        void Register<TInterface, TImplementation>();

        void Register(Type tInterface, Type tImplementation);

        void Register<TImplementation>() where TImplementation : class;

        void RegisterSingleton<TInterface, TImplementation>();

        void RegisterSingleton(Type tInterface, Type tImplementation);

        void RegisterSingleton<TImplementation>() where TImplementation : class;

        IEnumerable<ConfiguratedType> GetConfiguratedTypes(Type type);

        ConfiguratedType GetConfiguratedType(Type tinterface);
    }
}
