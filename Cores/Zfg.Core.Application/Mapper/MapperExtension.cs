using T = AutoMapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using Zfg.Core.Mapper;
using Autofac;

namespace Zfg.Core.Application
{
    public static class MapperExtension
    {
        public static IEngine UseMapper(this IEngine engine, string basePath = "", params Assembly[] assemblies)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }

            var configura = new T.MapperConfiguration(cfg =>
            {
                var builder = new MapperConfigBuilder(cfg);
                var stormdlls = System.IO.Directory.GetFiles(basePath, "*.dll");
                foreach (var f in stormdlls)
                {
                    if (!File.Exists(f))
                    {
                        continue;
                    }
                    try
                    {
                        var storm = Assembly.Load(System.IO.Path.GetFileName(f).Replace(".dll", ""));
                        AutoAssemble(engine, storm, builder);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(f.ToString());
                        Console.WriteLine(ex);
                    }
                }

                if (assemblies != null)
                {
                    foreach (var ass in assemblies)
                    {
                        AutoAssemble(engine, ass, builder);
                    }
                }
            });

            engine.Register<SysMapper, IMapper>(() => new SysMapper(configura.CreateMapper()), lift: LiftTime.Single);
            return engine;
        }

        private static void AutoAssemble(IEngine engine, Assembly storm, IMapperConfigBuider buider)
        {
            var typeList = storm.GetTypes();
            foreach (var t in typeList)
            {
                if (t.IsAssignableTo<IMapperCfgFactory>() && !t.IsAbstract)
                {
                    try
                    {
                        var mapper = System.Activator.CreateInstance(t) as IMapperCfgFactory;
                        if (mapper != null)
                        {
                            mapper.Create(buider);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
        }

    }
}
