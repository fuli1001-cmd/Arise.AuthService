using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Serilog;

namespace AuthService
{
    public class Program
    {
        public static readonly string AppName = "AuthService";

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(hostBuilderContext =>
                {
                    var endpointConfiguration = new EndpointConfiguration("authservice");
                    //var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    //transport.ConnectionString("host=rabbitmq");
                    transport.ConnectionString("host=43.225.159.87");
                    transport.UseConventionalRoutingTopology();
                    endpointConfiguration.EnableInstallers();

                    //endpointConfiguration.SendFailedMessagesTo("error");
                    //endpointConfiguration.AuditProcessedMessagesTo("audit");
                    //endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");
                    //var metrics = endpointConfiguration.EnableMetrics();
                    //metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

                    return endpointConfiguration;
                })
                .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("ApplicationContext", AppName)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/.txt"), rollingInterval: RollingInterval.Day)
                    //.WriteTo.Seq("http://seq")
                    //.ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .CreateLogger();
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
