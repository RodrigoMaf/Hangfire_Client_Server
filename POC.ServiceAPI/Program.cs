using System;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace POC.ServiceAPI
{
    /// <summary>Ponto de entrada do sistema</summary>
    public class Program
    {
        /// <summary>Método da inicio do sistema</summary>
        /// <param name="args">Argumentos passados para o serviço por cmd</param>
        public static void Main(string[] args)
        {
            Thread.Sleep(20000);
            ////Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>Cria um host do serviço</summary>
        /// <param name="args">Argumentos passados para o serviço por cmd</param>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                config
                    .SetBasePath(path)
                    .AddCommandLine(args)
                    .AddJsonFile("ConfigFiles/appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"ConfigFiles/appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                    .AddJsonFile("ConfigFiles/Infra/Security.json", optional: false)
                    .AddJsonFile("ConfigFiles/Serilog.json", optional: false)
                    ////.AddJsonFile($"ConfigFiles/Serilog.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                    .AddJsonFile("ConfigFiles/Swagger.json", optional: false)
                    .AddJsonFile("ConfigFiles/mvc-options.json", optional: false)
                    .AddJsonFile("ConfigFiles/hangfire-config.json", optional: false)

                    .AddEnvironmentVariables("ASPNETCORE_");
            })
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom
                        .Configuration(hostingContext.Configuration, "Serilog")
                        .CreateLogger()
                );
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                //.UseKestrel((p) => p.AddServerHeader = false)
                ;
            });
    }
}
