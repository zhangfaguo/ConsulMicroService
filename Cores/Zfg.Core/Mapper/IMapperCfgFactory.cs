using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.Mapper;

namespace Zfg.Core
{
    public interface IMapperCfgFactory
    {
        void Create(IMapperConfigBuider buider);
    }
}
