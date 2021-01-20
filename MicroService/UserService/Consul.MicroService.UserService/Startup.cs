using Autofac;
using Consul.MicroService.UserService.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
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

            //services.AddAuthentication("oidc")
            //         .AddOpenIdConnect("oidc", options =>
            //           {
            //               options.Authority = "http://localhost:7001";
            //               options.ClientId = "mvc";
            //               options.ClientSecret = "mvc";
            //               options.ResponseType = "code";
            //               options.RequireHttpsMetadata = false;
            //               options.Scope.Add("api");
            //        });


            services.AddAuthentication("Bearer")
                 .AddJwtBearer("Bearer", options =>
                 {
                     options.Authority = "http://192.168.1.60:7001";
                     options.RequireHttpsMetadata = false;
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateAudience = false,
                     };
                 });

            // adds an authorization policy to make sure the token is for scope 'api1'
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api");

                });
            });
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", "api");

            //    });
            //});
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
            app.Use(req =>
            {
                return async (context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(context.Request.Path);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (var item in context.Request.Headers)
                    {
                        Console.WriteLine($"{item.Key}:    {item.Value.FirstOrDefault()}");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    var originalBodyStream = context.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;
                        await req(context);
                        responseBody.Position = 0;
                        var sr = new StreamReader(responseBody);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(sr.ReadToEnd());
                        Console.ForegroundColor = ConsoleColor.White;
                        responseBody.Seek(0, SeekOrigin.Begin);

                        await responseBody.CopyToAsync(originalBodyStream);
                        context.Response.Body = originalBodyStream;
                    }
                };
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseConsul(Configuration);
        }
    }
}
