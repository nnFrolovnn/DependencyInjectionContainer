using DependencyInjectionContainer.Interfaces;
using System;
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


        public T Resolve<T>() where T : class
        {
            return (T)Create(typeof(T));
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

            return null;
        }

    }
}
