using DependencyInjectionContainer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer
{
    public class DIConfiguration : IDIConfiguration
    {
        private readonly ConcurrentDictionary<Type, ConfiguratedType> dictionary;

        IDictionary<Type, ConfiguratedType> RegisteredTypesDictionary => dictionary;

        public DIConfiguration()
        {
            dictionary = new ConcurrentDictionary<Type, ConfiguratedType>();
        }

        #region public Registration Methods

        public void Register<TImplementation>() where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation));
        }

        public void Register<TInterface, TImplementation>()
        {
            RegisterType(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type tInterface, Type tImplementation)
        {
            RegisterType(tInterface, tImplementation);
        }    
  
        public void RegisterSingleton<TInterface, TImplementation>()
        {
            RegisterType(typeof(TInterface), typeof(TImplementation), true);
        }

        public void RegisterSingleton(Type tInterface, Type tImplementation)
        {
            RegisterType(tInterface, tImplementation, true);
        }

        public void RegisterSingleton<TImplementation>() where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation), true);
        }

        #endregion

        private void RegisterType(Type tInterface, Type tImplementation, bool isSingleton = false)
        {
            if (!tImplementation.IsInterface && !tImplementation.IsAbstract)
            {
                ConfiguratedType configuratedType = new ConfiguratedType(tImplementation, tInterface);

                if (!dictionary.Values.Contains(configuratedType))
                {
                    dictionary.TryAdd(tInterface, configuratedType);
                }
                else
                {
                    throw new Exception($"can't add value to {nameof(dictionary)}");
                }
            }
            else
            {
                throw new Exception($"{nameof(tImplementation)} can't be an interface or abstract");
            }
        }

        public ConfiguratedType GetConfiguratedType(Type tinterface)
        {
            return (dictionary.TryGetValue(tinterface, out var value)) ? value : null;
        }

        public IEnumerable<ConfiguratedType> GetConfiguratedTypes(Type type)
        {
            return dictionary.TryGetValue(type, out var configuratedType)? (IEnumerable<ConfiguratedType>)configuratedType: null;
        }
    }
}
