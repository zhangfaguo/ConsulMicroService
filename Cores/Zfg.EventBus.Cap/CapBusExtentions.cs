using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Zfg.Core.EventBus.Cap;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DotNetCore.CAP.Internal;

namespace Zfg.Core
{
    public static class CapBusExtentions
    {

        internal static IServiceCollection ServiceCollection;

        public static EventBusBuilder UseCap<Ef>(this EventBusBuilder builder, IConfiguration configuration)
            where Ef : DbContext
        {
            ServiceCollection = builder.Engine;
            builder.Engine.AddCap(options =>
            {
                options.UseRabbitMQ(opt=> {
                    opt.HostName = configuration.GetSection("cap:mqHost").Value;
                    opt.UserName = configuration.GetSection("cap:mqUserName").Value;
                    opt.Password = configuration.GetSection("cap:mqPassWord").Value;
                    opt.Port = configuration.GetSection("cap:mqPort").Value.ToInt();
                });
                options.UseEntityFramework<Ef>(opt =>
                {
                    opt.TableNamePrefix = configuration.GetSection("cap:pre").Value;
                });
                options.RegisterExtension(new CapOptionsExtension());
            });
            builder.Engine.AddScoped<IPublish, CapPublish>();
            return builder;
        }
    }

    public class CapOptionsExtension : ICapOptionsExtension
    {
        public void AddServices(IServiceCollection services)
        {
            services.Replace(new ServiceDescriptor(typeof(IConsumerServiceSelector), typeof(CapCustomerSelector), ServiceLifetime.Singleton));
        }
    }
}
