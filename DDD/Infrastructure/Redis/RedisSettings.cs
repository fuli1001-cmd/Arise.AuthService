using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Infrastructure.Redis
{
    public class RedisSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}
