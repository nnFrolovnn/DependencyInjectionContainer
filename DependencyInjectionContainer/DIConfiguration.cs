﻿using DependencyInjectionContainer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DependencyInjectionContainer
{
    public class DIConfiguration : IDIConfiguration
    {
        private readonly ConcurrentDictionary<Type, ConfiguratedType> dictionary;

        public DIConfiguration()
        {
            dictionary = new ConcurrentDictionary<Type, ConfiguratedType>();
        }

        public IEnumerable<ConfiguratedType> GetConfiguratedTypes(Type type)
        {
            return dictionary?.Values;
        }

        public void Register<TImplementation>() where TImplementation : class
        {
            Register(typeof(TImplementation), typeof(TImplementation));
        }

        public void Register<TInterface, TImplementation>()
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type tInterface, Type tImplementation)
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
    }
}
