﻿using Consul.MicroServer.Client.Models;
using Consul.MicroService.UserService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Consul.MicroServer.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IUser AppUser { get; }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
          //  AppUser = appUser;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(Content("12333"));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
