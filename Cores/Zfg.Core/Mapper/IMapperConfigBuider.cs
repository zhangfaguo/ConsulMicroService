using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Zfg.Core.Mapper
{
    public interface IMapperConfigBuider
    {
        void Create<Ts, Ti>();

        void Create<Ts, Ti>(Dictionary<string, Expression<Func<Ts, object>>> conveters, Dictionary<string, Expression<Func<Ti, object>>> tooto);
    }
}
