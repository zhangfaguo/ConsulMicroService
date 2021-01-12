using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Application.Ioc
{
    public class ScopeFactory
    {
        public static IScope Create(ILifetimeScope scope)
        {
            return new ScopeProxy(scope);
        }
    }
}
