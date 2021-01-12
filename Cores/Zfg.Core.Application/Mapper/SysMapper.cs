
using System;
using System.Collections.Generic;
using System.Text;
using T = AutoMapper;

namespace Zfg.Core.Mapper
{
    internal class SysMapper : IMapper
    {
        T.IMapper innerMapper;

        public SysMapper(T.IMapper mapper)
        {
            innerMapper = mapper;
        }

        public T MapTo<T>(object obj)
            where T : class
        {
            if (obj == null) return default(T);

            return innerMapper.Map<T>(obj);
        }
    }
}
