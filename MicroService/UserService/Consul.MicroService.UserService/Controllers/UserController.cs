using Consul.MicroService.UserService.AppService.Contract;
using Microsoft.AspNetCore.Authorization;
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

       [Authorize]
        [HttpGet("index")]
        public dynamic Index()
        {
            var user = User;
            return Dns.GetHostName();
        }


        [HttpGet("search")]
        public dynamic Get(int id)
        {
            return AppUser.GetUser(id);
        }

    }
}
