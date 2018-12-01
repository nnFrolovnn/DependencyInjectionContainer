using DependencyInjectionContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DIContainer : IDIContainer
    {
        public T Resolve<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
