using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core;
using Zfg.Core.Application.Logs;

namespace Zfg.Core.Application
{
    public static class LoggerExtensions
    {
        public static IEngine UseLogger(this IEngine engine)
        {
            engine.RegisterType<NLogger, ILogger>(lift: LiftTime.RequestSingle);
            return engine;
        }
    }
}
