using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public interface IMapper
    {

        T MapTo<T>(object obj) where T : class;

    }
}
