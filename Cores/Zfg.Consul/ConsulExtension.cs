using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Zfg.Consul
{
    public static class ConsulExtension
    {
        /// <summary>
        /// Add Consul
        /// 添加consul
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulCfg = GetConfig(configuration);
            if (consulCfg?.Enable == true)
            {
                services.AddSingleton<IConsulClient>(sp => new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{consulCfg.ConsulCenterIp}:{consulCfg.ConsulCenterPort}");
                }));
            }
            return services;
        }


        /// <summary>
        /// use Consul
        /// 使用consul
        /// The default health check interface format is http://host:port/HealthCheck
        /// 默认的健康检查接口格式是 http://host:port/HealthCheck
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {

            var consulCfg = GetConfig(configuration);
            if (consulCfg?.Enable == true)
            {
                IConsulClient consul = app.ApplicationServices.GetRequiredService<IConsulClient>();
                var appLife = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
                Console.WriteLine($"ClientPort:{consulCfg.ClientPort}");
                Console.WriteLine($"ClientIp:{consulCfg.ClientIp}");
                //register localhost address
                var scaml = consulCfg.RequireHttps ? "https" : "http";
                //注册本地地址
                var localhostregistration = new AgentServiceRegistration()
                {

                    Checks = new[] { new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                        Interval = TimeSpan.FromSeconds(30),
                        HTTP = $"{scaml}://{consulCfg.ClientIp}:{consulCfg.ClientPort}/HealthCheck",
                    } },
                    Address = consulCfg.ClientIp,
                    ID = $"{consulCfg.ServerName}-{consulCfg.ClientIp}-{consulCfg.ClientPort}",
                    Name = consulCfg.ServerName,
                    Port = consulCfg.ClientPort,


                };

                consul.Agent.ServiceRegister(localhostregistration).GetAwaiter().GetResult();
                var key = $"upstreams/{consulCfg.ServerName}/{consulCfg.ClientIp}:{consulCfg.ClientPort}".ToLower();
                consul.KV.Put(new KVPair(key));
                ////send consul request after service stop
                ////当服务停止后想consul发送的请求
                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(localhostregistration.ID).GetAwaiter().GetResult();
                    consul.KV.Delete(key);
                });

                app.Map("/HealthCheck", s =>
                {
                    s.Run(async context =>
                    {
                        await context.Response.WriteAsync("ok");
                    });
                });
            }


            return app;
        }

        private static ConsulConfig GetConfig(IConfiguration configuration)
        {
            return configuration.GetSection("consul").Get<ConsulConfig>();
        }
    }
}
