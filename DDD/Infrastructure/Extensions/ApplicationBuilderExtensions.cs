using Arise.DDD.Infrastructure.ServiceDiscovery;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void RegisterWithConsul(this IApplicationBuilder app,
            IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>();
            consulClient.RegisterService(consulConfig.Value);
            lifetime.ApplicationStopping.Register(() => consulClient.DeregisterService(consulConfig.Value));
        }
    }
}
