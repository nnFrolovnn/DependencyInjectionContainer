using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.Interfaces
{
    public interface IDIContainer
    {
        T Resolve<T>() where T : class;
    }
}
