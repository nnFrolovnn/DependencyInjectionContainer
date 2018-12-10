using System;

namespace DependencyInjectionContainer
{
    public class ConfiguratedType
    {
        public bool IsSingleton { get; set; }
        
        public Type GetImplementationInterface { get; }

        public Type GetImplementation { get; }

        public object GetInstance { get; set; }

        public ConfiguratedType (Type impl, Type interf, bool isSingleton = false)
        {
            IsSingleton = isSingleton;
            GetImplementation = impl;
            GetImplementationInterface = interf;
            GetInstance = null;
        }
    }
}
