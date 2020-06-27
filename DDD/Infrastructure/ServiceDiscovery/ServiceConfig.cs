using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Infrastructure.ServiceDiscovery
{
    public class ServiceConfig
    {
        public string ServiceDiscoveryAddress { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
    }
}
