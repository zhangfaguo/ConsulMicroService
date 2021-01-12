using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public interface IScope : IDisposable
    {
        T Resolve<T>();

        T Resolve<T>(string name);

        void Releas();
    }
}
