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
        private static object obj = new object();
        private readonly IDIConfiguration container;
        private readonly ConcurrentStack<Type> stack;
        private Type currentType;

        public DIContainer(IDIConfiguration dIConfiguration)
        {
            if (Validate(dIConfiguration))
            {
                container = dIConfiguration;
                stack = new ConcurrentStack<Type>();
            }
            else
            {
                throw new Exception("IDConfiguration has wrong structure");
            }
        }

        private bool Validate(IDIConfiguration configuration)
        {
            if(configuration != null)
            {
                IDictionary<Type, ConfiguratedType> typesDictionary = configuration.RegisteredTypesDictionary;
                foreach(var pair in typesDictionary)
                {
                    if(pair.Value.GetImplementation.IsAbstract || pair.Value.GetImplementation.IsInterface)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
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
                currentType = type;
                return (T)RetrieveInst(registeredType);
            }
        
            throw new Exception("no such type");
        }

        private object Create(Type type)
        {
            var configuratedType = container.GetConfiguratedType(type);

            if (configuratedType != null)
            {
                if (!stack.Contains(configuratedType.GetImplementationInterface))
                {
                    stack.Push(configuratedType.GetImplementationInterface);

                    var instanceType = configuratedType.GetImplementation;
                    if (instanceType.IsGenericTypeDefinition)
                    {
                        instanceType = instanceType.MakeGenericType(currentType.GenericTypeArguments);
                    }

                    var constructors = instanceType.GetConstructors().OrderByDescending
                        (x => x.GetParameters().Length).ToArray();

                    bool isCreated = false;
                    int constructorNumber = 1;
                    object result = null;

                    while (!isCreated && constructorNumber <= constructors.Count())
                    {
                        try
                        {
                            var useConstructor = constructors[constructorNumber - 1];
                            var param = GetConstructorParam(useConstructor);
                            result = Activator.CreateInstance(instanceType, param);
                            isCreated = true;
                        }
                        catch
                        {
                            isCreated = false;
                            constructorNumber++;
                        }
                    }

                    if (!stack.TryPop(out var temp) || temp != configuratedType.GetImplementationInterface)
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
                    throw new Exception($"can't resolve type{stack.FirstOrDefault().Name}");
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

        private object[] GetConstructorParam(ConstructorInfo constructor)
        {
            var parametersInfo = constructor.GetParameters();
            var parameters = new object[parametersInfo.Length];

            for (int i = 0; i < parametersInfo.Length; i++)
            {
                parameters[i] = RetrieveInst(container.GetConfiguratedType(parametersInfo[i].ParameterType));
            }

            return parameters;
        }

        private object RetrieveInst(ConfiguratedType registeredType)
        {
            if (registeredType != null)
            {
                if (registeredType.IsSingleton)
                {
                    if(registeredType.GetInstance == null)
                    {
                        lock(obj)
                        {
                            if(registeredType.GetInstance == null)
                            {
                                registeredType.GetInstance = Create(registeredType.GetImplementationInterface);
                            }
                        }
                    }
                    else
                    {
                        return registeredType.GetInstance;
                    }
                }


                object createdInst = Create(registeredType.GetImplementationInterface);
                return createdInst;
            }

            throw new Exception("can't resolve this type");
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return (IEnumerable<T>)CreateIEnumerable(typeof(T));          
        }
    }
}
