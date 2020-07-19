using Arise.DDD.Infrastructure.ServiceDiscovery;
using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Extensions
{
    public static class ConsulClientExtensions
    {
        public static void RegisterService(this IConsulClient consulClient, ConsulConfig consulConfig)
        {
            var registrationId = $"{consulConfig.ServiceName}-{consulConfig.ServiceId}";
            var serviceAddress = new Uri(consulConfig.ServiceAddress);

            var registration = new AgentServiceRegistration
            {
                ID = registrationId,
                Name = consulConfig.ServiceName,
                Address = serviceAddress.Host,
                Port = serviceAddress.Port
            };

            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();
        }

        public static void DeregisterService(this IConsulClient consulClient, ConsulConfig consulConfig)
        {
            var registrationId = $"{consulConfig.ServiceName}-{consulConfig.ServiceId}";
            consulClient.Agent.ServiceDeregister(registrationId).Wait();
        }
    }
}
