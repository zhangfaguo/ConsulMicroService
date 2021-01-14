using Consul.MicroService.UserService.AppService.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Consul.MicroService.UserService.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        IAppUserService AppUser { get; }


        public UserController(IAppUserService appUser)
        {
            AppUser = appUser;
        }


        [HttpGet("index")]
        public dynamic Index()
        {
            return Dns.GetHostName();
        }


        [HttpGet("get")]
        public dynamic Get()
        {
            return Dns.GetHostName();
        }

    }
}
