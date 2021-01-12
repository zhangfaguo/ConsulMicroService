using Consul.MicroService.UserService.AppService.Contract.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consul.MicroService.UserService.AppService.Contract
{
    public interface IAppUserService
    {

        UserModel GetUser(int id);
    }
}
