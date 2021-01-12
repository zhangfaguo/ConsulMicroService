using Consul.MicroService.UserService.AppService.Contract;
using Consul.MicroService.UserService.AppService.Contract.Model;
using Consul.MicroService.UserService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core;
using System.Linq;

namespace Consul.MicroService.UserService.AppService
{
    [Ioc]
    public class AppUserService : Life, IAppUserService
    {
        IRepository<K_User> DbUser { get; }
        IMapper Mapper { get; }

        public AppUserService(IRepository<K_User> user, IMapper mapper)
        {
            DbUser = user;
            Mapper = mapper;
        }

        public UserModel GetUser(int id)
        {
            return DbUser.FirstOrDefault(t => t.Id == id).MapTo<UserModel>(Mapper);
        }
    }
}
