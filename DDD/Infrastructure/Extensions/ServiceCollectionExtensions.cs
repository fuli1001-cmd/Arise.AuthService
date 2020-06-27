using Arise.DDD.Infrastructure.ServiceDiscovery;
using Consul;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arise.DDD.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlDataAccessServices<T>(this IServiceCollection services, 
            string connectionString, string migrationsAssembly)
            where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });
        }

        public static IConsulClient AddConsulClient(this IServiceCollection services, ServiceConfig serviceConfig)
        {
            var consulClient = new ConsulClient(config =>
            {
                config.Address = new Uri(serviceConfig.ServiceDiscoveryAddress);
            });

            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);

            return consulClient;
        }
    }
}
