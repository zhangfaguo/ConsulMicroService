using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.EventBus
{
    public class EventBusBuilder
    {
        public IPublish Publisher { get; }


        public IServiceCollection Engine { get; set; }

    }
}
