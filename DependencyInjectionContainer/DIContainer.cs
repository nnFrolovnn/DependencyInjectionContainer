using DependencyInjectionContainer.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DIContainer : IDIContainer
    {
        private readonly IDIConfiguration container;
        private readonly ConcurrentStack<Type> stack;

        public DIContainer(IDIConfiguration dIConfiguration)
        {
            container = dIConfiguration;
            stack = new ConcurrentStack<Type>();
        }

        private bool Validate(IDIConfiguration configuration)
        {

        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);     
            var registeredType = container.GetConfiguratedType(type);
            if (registeredType == null && type.IsGenericType)
            {
                registeredType = container.GetConfiguratedType(type.GetGenericTypeDefinition());
            }

            if (registeredType != null)
            {
                return (T)RetrieveInst(registeredType);
            }

            throw new Exception("no such type");
        }

        private object Create(Type type)
        {
            var configuratedType = container.GetConfiguratedType(type);

            if (configuratedType != null)
            {
                if(configuratedType.GetInstance != null && configuratedType.IsSingleton)
                {
                    return configuratedType.GetInstance;
                }

                if (!stack.Contains(configuratedType.GetImplementation))
                {
                    stack.Push(configuratedType.GetImplementation);

                    var constructors = configuratedType.GetImplementation.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();

                    bool isCreated = false;
                    int constructorNumber = 1;
                    object result = null;

                    while (!isCreated && constructorNumber <= constructors.Count())
                    {
                        try
                        {
                            var useConstructor = constructors[constructorNumber - 1];
                            result = CreatefromConstructor(useConstructor);
                            isCreated = true;
                        }
                        catch
                        {
                            isCreated = false;
                            constructorNumber++;
                        }
                    }

                    if (!stack.TryPop(out var temp) || temp != configuratedType.GetImplementation)
                    {
                        throw new Exception("can't correctly pop element from stack");
                    }

                    if (isCreated)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception($"{type.FullName} can't be created");
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception($"{type.FullName} is not registered");
            }
        }

        private object CreateIEnumerable(Type type)
        {
            var innerType = type.GenericTypeArguments.FirstOrDefault();
            var registeredType = container.GetConfiguratedType(innerType);

            if (registeredType != null)
            {
                var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(innerType));

                var registeredTypes = container.GetConfiguratedTypes(innerType);
                foreach (var item in registeredTypes)
                {
                    collection.Add(RetrieveInst(item));
                }

                return collection;
            }
            else
            {
                throw new Exception($"Not registered type: {innerType?.FullName}");
            }         
        }

        private object CreatefromConstructor(ConstructorInfo constructor)
        {
            var parametersInfo = constructor.GetParameters();
            var parameters = new object[parametersInfo.Length];

            for (int i = 0; i < parametersInfo.Length; i++)
            {
                parameters[i] = RetrieveInst(container.GetConfiguratedType(parametersInfo[i].ParameterType));
            }

            return constructor.Invoke(parameters);
        }

        private object RetrieveInst(ConfiguratedType registeredType)
        {
            if (registeredType != null)
            {
                if (registeredType.IsSingleton &&
                    registeredType.GetInstance != null)
                {
                    return registeredType.GetInstance;
                }

                object crestedInst = Create(registeredType.GetImplementationInterface);

                registeredType.GetInstance = crestedInst;
                return crestedInst;
            }

            throw new Exception("can't resolve this type");
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return (IEnumerable<T>)CreateIEnumerable(typeof(T));          
        }
    }
}
