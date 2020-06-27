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
        public static async Task RegisterServicesAsync(this IConsulClient consulClient, ServiceConfig serviceConfig)
        {
            var registrationId = $"{serviceConfig.ServiceName}-{serviceConfig.ServiceId}";
            var serviceAddress = new Uri(serviceConfig.ServiceAddress);

            var registration = new AgentServiceRegistration
            {
                ID = registrationId,
                Name = serviceConfig.ServiceName,
                Address = serviceAddress.Host,
                Port = serviceAddress.Port
            };

            await consulClient.Agent.ServiceDeregister(registration.ID);
            await consulClient.Agent.ServiceRegister(registration);
        }

        public static async Task DeregisterServicesAsync(this IConsulClient consulClient, ServiceConfig serviceConfig)
        {
            var registrationId = $"{serviceConfig.ServiceName}-{serviceConfig.ServiceId}";
            await consulClient.Agent.ServiceDeregister(registrationId);
        }
    }
}
