using Autofac;
using Consul.MicroService.UserService.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zfg.Consul;
using Zfg.Core;
using Zfg.Core.Application;

namespace Consul.MicroService.UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder container)
        {
            Engine.Instance.builder = container;
            Engine.Instance.UseLogger()
                           .LoadService()
                           .UseMapper()
                           .UseRepository();
            Engine.Instance.Register<IConfiguration, IConfiguration>(() => Configuration, lift: LiftTime.Single);

            Engine.Instance.Register<DbContextOptions<EfContent>, DbContextOptions>(() =>
            {
                var builder = new DbContextOptionsBuilder<EfContent>();
                var conntectString = Configuration.GetConnectionString("content");
                builder.UseMySql(conntectString, ServerVersion.AutoDetect(conntectString));
                return builder.Options;
            }, lift: LiftTime.Single);

            Engine.Instance.RegisterType<EfContent, DbContext>();

            container.RegisterBuildCallback(lifeScope =>
            {
                Engine.Instance.container = lifeScope as IContainer;
            });
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.Configuration = new XmlLoggingConfiguration(basePath + "nlog.config");
            services.AddDbContext<EfContent>((cfg) =>
            {
                var conntectString = Configuration.GetConnectionString("content");
                cfg.UseMySql(conntectString, ServerVersion.AutoDetect(conntectString));
            });
            services.AddConsul(Configuration);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consul.MicroService.UserService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consul.MicroService.UserService v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseConsul(Configuration);
        }
    }
}
