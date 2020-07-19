using Arise.DDD.Infrastructure.ServiceDiscovery;
using Consul;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public static IConsulClient AddConsulClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulConfig>(configuration.GetSection("ConsulConfig"));

            var consulClient = new ConsulClient(config =>
            {
                var address = configuration["ConsulConfig:ServiceDiscoveryAddress"];
                config.Address = new Uri(address);
            });

            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);

            return consulClient;
        }
    }
}
