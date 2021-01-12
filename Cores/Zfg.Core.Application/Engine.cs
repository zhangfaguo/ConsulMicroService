using Autofac;
using Autofac.Core.Resolving.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.Application.Ioc;

namespace Zfg.Core.Application
{
    public class Engine : IEngine
    {

        public IContainer container = null;

        public ContainerBuilder builder = null;

        #region Instance
        static Engine()
        {
            Engine._instance = new Engine();
            IocManager.Instance = Engine._instance;
        }
        private static Engine _instance;
        public static Engine Instance
        {
            get
            {
                return Engine._instance;
            }
        }
        #endregion

        #region IOC

        public Engine InitIoc()
        {

            builder = new ContainerBuilder();
            return this;
        }

        public void Build()
        {
            container = builder.Build();
        }

        public IScope RootScope
        {
            get
            {
                return new ScopeProxy(container);
            }

        }

        public IScope NewScope()
        {
            var scope = container.BeginLifetimeScope();
            return new ScopeProxy(scope);

        }



        public IEngine RegisterType<TS, TI>(string name = "", LiftTime lift = LiftTime.RequestSingle)
          where TS : TI
          where TI : class
        {
            return Register(typeof(TS), typeof(TI), name, lift);
        }

        public IEngine RegisterType<TS>(string name = "", LiftTime lift = LiftTime.RequestSingle)
          where TS : class
        {
            return Register(typeof(TS), null, name, lift);
        }


        public IEngine Register<TS>(string name = "", LiftTime lift = LiftTime.RequestSingle)
         where TS : class
        {
            return Register(typeof(TS), typeof(TS), name, lift);
        }

        public IEngine Register<Ts, Ti>(Func<Ts> fnc, string name = "", LiftTime lift = LiftTime.RequestSingle)
            where Ti : class
            where Ts : Ti
        {
            var register = builder.Register((c) =>
            {
                return fnc();
            });


            if (!string.IsNullOrEmpty(name))
            {
                register = register.Named(name, typeof(Ti));
            }
            else
            {
                register = register.As<Ti>();
            }

            switch (lift)
            {
                case LiftTime.RequestSingle:
                    register = register.InstancePerLifetimeScope();
                    break;
                case LiftTime.Single:
                    register = register.SingleInstance();
                    break;
            }

            register.OnActivated((s) =>
            {
                if (s.Instance is ILife ser)
                {
                    var instanceLook = s.Context as ResolveRequestContext;
                    if (instanceLook != null)
                    {
                        ser.Scope = new ScopeProxy(instanceLook.ActivationScope);
                    }
                }

            });
            return this;
        }

        public IEngine Register(Type sourceType, Type interfaceType, string name = "", LiftTime lift = LiftTime.RequestSingle)
        {

            var register = builder.RegisterType(sourceType);
            if (!string.IsNullOrEmpty(name))
            {
                register = register.Named(name, interfaceType);
            }
            else if (interfaceType != null)
            {
                register = register.As(interfaceType);
            }
            else
            {
                register = register.AsImplementedInterfaces();
            }

            switch (lift)
            {
                case LiftTime.RequestSingle:
                    register = register.InstancePerLifetimeScope();
                    break;
                case LiftTime.Single:
                    register = register.SingleInstance();
                    break;
            }


            register.OnActivated((s) =>
            {
                if (s.Instance is ILife ser)
                {
                    var instanceLook = s.Context as ResolveRequestContext;
                    if (instanceLook != null)
                    {
                        ser.Scope = new ScopeProxy(instanceLook.ActivationScope);
                    }
                }

            });

            return this;
        }


        public IEngine RegisterGeneric(Type sourceType, Type interfaceType, string name = "", LiftTime lift = LiftTime.RequestSingle)
        {

            var register = builder.RegisterGeneric(sourceType);
            if (!string.IsNullOrEmpty(name))
            {
                register = register.Named(name, interfaceType);
            }
            else if (interfaceType != null)
            {
                register = register.As(interfaceType);
            }
            else
            {
                register = register.AsImplementedInterfaces();
            }

            switch (lift)
            {
                case LiftTime.RequestSingle:
                    register = register.InstancePerLifetimeScope();
                    break;
                case LiftTime.Single:
                    register = register.SingleInstance();
                    break;
            }


            register.OnActivated((s) =>
            {
                if (s.Instance is ILife ser)
                {
                    var instanceLook = s.Context as ResolveRequestContext;
                    if (instanceLook != null)
                    {
                        ser.Scope = new ScopeProxy(instanceLook.ActivationScope);
                    }
                }

            });

            return this;
        }



        #endregion
    }
}
