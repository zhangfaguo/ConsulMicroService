using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.EventBus;

namespace Zfg.Core
{
    public static class EventBusExtentions
    {

        public static IServiceCollection AddEventBus(this IServiceCollection engine, Action<EventBusBuilder> builderAct)
        {
            var builder = new EventBusBuilder();
            builder.Engine = engine;
            builderAct(builder);
            return engine;
        }


    }
}
