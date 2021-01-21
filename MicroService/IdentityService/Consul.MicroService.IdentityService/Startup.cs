using Consul.MicroService.IdentityService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Zfg.Consul;

namespace Consul.MicroService.IdentityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectStr = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectStr, ServerVersion.AutoDetect(connectStr), sql => sql.MigrationsAssembly(migrationsAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
            })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                     .AddConfigurationStore(options =>
                     {
                         options.ConfigureDbContext = builder =>
                         {
                             builder.UseMySql(connectStr, ServerVersion.AutoDetect(connectStr), sql => sql.MigrationsAssembly(migrationsAssembly));
                         };
                     })
                     .AddOperationalStore(options =>
                     {
                         options.ConfigureDbContext = builder => builder.UseMySql(connectStr, ServerVersion.AutoDetect(connectStr), sql => sql.MigrationsAssembly(migrationsAssembly));
                     })
                     .AddAspNetIdentity<IdentityUser>()
                     .AddDeveloperSigningCredential();
            services.AddControllersWithViews();
            services.AddAuthentication();
            services.AddConsul(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
           // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.Use(req =>
            {
                return async (context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(context.Request.Path);
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
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseConsul(Configuration);

        }
    }
}
