using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zfg.Core.Mapper;

namespace Zfg.Core.Application
{
    internal class MapperConfigBuilder : IMapperConfigBuider
    {
        IMapperConfigurationExpression expression;

        public MapperConfigBuilder(IMapperConfigurationExpression _expression)
        {
            expression = _expression;
        }


        public void Create<Ts, Ti>()
        {
            expression.CreateMap<Ts, Ti>();
            expression.CreateMap<Ti, Ts>();
        }


        public void Create<Ts, Ti>(Dictionary<string, Expression<Func<Ts, object>>> conveters, Dictionary<string, Expression<Func<Ti, object>>> tooto)
        {
            var exp = expression.CreateMap<Ts, Ti>();
            foreach (var item in conveters)
            {
                exp.ForMember(item.Key, t => t.MapFrom(item.Value));
            }

            var exp2 = expression.CreateMap<Ti, Ts>();

            foreach (var item in tooto)
            {
                exp2.ForMember(item.Key, t => t.MapFrom(item.Value));
            }
        }
    }
}
