using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Application.Ioc
{
    internal class ScopeProxy : IScope
    {
        ILifetimeScope scope;

        public ScopeProxy(ILifetimeScope scope)
        {
            this.scope = scope;
        }


        public T Resolve<T>()
        {
            return scope.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return scope.ResolveNamed<T>(name);
        }


        public void Releas()
        {
            scope.Dispose();
            scope = null;
        }

        public void Dispose()
        {
            Releas();
        }

    }
}
