using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.Db;

namespace Zfg.Core
{
    public static class DbExtension
    {
        public static IEngine UseRepository(this IEngine engine)
        {
            engine.RegisterGeneric(typeof(SQLRepository<>), typeof(IRepository<>));
            return engine;
        }
    }
}
