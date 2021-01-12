using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.Db;

namespace Consul.MicroService.UserService.Domain.Entity
{
    public class K_User : BaseEntity
    {

        public string RealName { get; set; }
    }
}
