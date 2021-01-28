using Consul.MicroService.UserService.AppService.Contract;
using Consul.MicroService.UserService.AppService.Contract.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using Zfg.Core.EventBus;

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

        [Authorize("ApiScope")]
        [HttpGet("index")]
        public dynamic Index()
        {
            var user = User;
            return Dns.GetHostName();
        }


        [HttpGet("search")]
        public dynamic Get(int id)
        {
            var user = AppUser.GetUser(id);
            HttpContext.RequestServices.GetRequiredService<IPublish>().Publish("user.show", user);
            return user;
        }

        [NonAction]
        [Subscribe("user.show")]
        public void Sub(UserModel user)
        {
            Console.WriteLine(user);
        }

    }
}
