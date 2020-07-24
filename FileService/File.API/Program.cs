using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FileService.File.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static ManualResetEvent CleanAppMre = new ManualResetEvent(true);
        public static ManualResetEvent CleanChatMre = new ManualResetEvent(true);
        public static ManualResetEvent StreamingMre = new ManualResetEvent(true);

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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
