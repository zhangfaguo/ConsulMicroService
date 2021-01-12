using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public abstract class Life : ILife
    {
        public IScope Scope { get; set; }
    }
}
