using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Zfg.Core.Application
{
    public static class ApplicationExtentions
    {
        public static IEngine LoadService(this IEngine engine, string basePath = "")
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }

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
                    AutoAssemble(engine, storm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(f.ToString());
                    Console.WriteLine(ex);
                }
            }

            return engine;
        }

        public static IEngine LoadAssemble(this IEngine engine, Assembly storm)
        {
            AutoAssemble(engine, storm);
            return engine;
        }


        private static void AutoAssemble(IEngine engine, Assembly storm)
        {
            var typeList = storm.GetTypes();
            foreach (var t in typeList)
            {

                var attrType = t.GetCustomAttribute(typeof(IocAttribute));
                if (attrType != null)
                {
                    var apiSummary = attrType as IocAttribute;
                    if (apiSummary != null)
                    {
                        engine.Register(t, apiSummary.InterfaceType, apiSummary.Name, apiSummary.LiftTime);
                    }
                }

            }
        }
    }
}
