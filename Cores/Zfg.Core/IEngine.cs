using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public interface IEngine
    {
        IScope NewScope();

        IScope RootScope { get; }

        IEngine RegisterType<TS, TI>(string name = "", LiftTime lift = LiftTime.RequestSingle)
         where TS : TI
         where TI : class;

        IEngine RegisterType<TS>(string name = "", LiftTime lift = LiftTime.RequestSingle)
         where TS : class;


        IEngine Register<TS>(string name = "", LiftTime lift = LiftTime.RequestSingle)
        where TS : class;

        IEngine Register<Ts, Ti>(Func<Ts> fnc, string name = "", LiftTime lift = LiftTime.RequestSingle)
            where Ti : class
            where Ts : Ti;

        IEngine Register(Type sourceType, Type interfaceType, string name = "", LiftTime lift = LiftTime.RequestSingle);

        IEngine RegisterGeneric(Type sourceType, Type interfaceType, string name = "", LiftTime lift = LiftTime.RequestSingle);
    }
}
