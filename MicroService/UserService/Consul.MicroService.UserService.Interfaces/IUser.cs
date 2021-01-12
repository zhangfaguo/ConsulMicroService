using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace Consul.MicroService.UserService.Interfaces
{

    public interface IUser
    {
        [HttpGet("userservice/user/index")]
        Task<string> Index();
    }
}
