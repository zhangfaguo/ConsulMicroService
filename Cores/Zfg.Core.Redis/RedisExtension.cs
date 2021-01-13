using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Zfg.Core.Common.Redis;
using Zfg.Core.Redis;

namespace Zfg.Core
{
    public static class RedisExtension
    {

        public static IEngine UseRedis(this IEngine engine, IConfiguration configuration)
        {
            engine.Register<RedisContent, IRedisContent>(() =>
            {
                return new RedisContent(new StackExchange.Redis.ConfigurationOptions
                {
                    Password = "",
                    SyncTimeout = 5000,
                    AbortOnConnectFail = false,
                    ConnectTimeout = 15000,
                    ResponseTimeout = 15000,
                    EndPoints = {
                         { "127.0.0.1",6379 }
                    },
                    AllowAdmin = true,
                });
            }, lift: LiftTime.Single);
            return engine;
        }
    }
}
