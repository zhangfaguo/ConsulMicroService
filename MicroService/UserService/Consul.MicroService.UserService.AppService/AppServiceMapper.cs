using Consul.MicroService.UserService.AppService.Contract.Model;
using Consul.MicroService.UserService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core;
using Zfg.Core.Mapper;

namespace Consul.MicroService.UserService.AppService
{
    public class AppServiceMapper : IMapperCfgFactory
    {
        public void Create(IMapperConfigBuider buider)
        {
            buider.Create<UserModel, K_User>();
        }
    }
}
