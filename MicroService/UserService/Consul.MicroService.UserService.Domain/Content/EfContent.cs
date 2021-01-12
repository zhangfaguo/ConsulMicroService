using Consul.MicroService.UserService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consul.MicroService.UserService.Domain
{
    public class EfContent : DbContext
    {

        public EfContent(DbContextOptions<EfContent> options) : base(options)
        {

        }


        public DbSet<K_User> Users { get; set; }
    }
}
