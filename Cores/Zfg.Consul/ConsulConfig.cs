using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Consul
{
    public class ConsulConfig
    {
        /// <summary>
        /// 启用Consul
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 注册中心IP
        /// </summary>
        public string ConsulCenterIp { get; set; }


        /// <summary>
        /// 注册中心端口
        /// </summary>
        public int ConsulCenterPort { get; set; }


        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }


        /// <summary>
        /// 服务注册IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 服务注册端口
        /// </summary>
        public int ClientPort { get; set; }
    }
}
