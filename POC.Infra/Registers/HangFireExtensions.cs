using System;
using System.Linq;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using POC.Infra.Hangfire;

namespace POC.Infra.Registers
{
    /// <summary>Configura o hangfire para uso do sistema</summary>
    public static class HangfireExtensions
    {
        /// <summary>Faz o registro de metricas no serviço DI</summary>
        /// <param name="services">Serviço DI</param>
        /// <param name="configuration">Configurações do sistema</param>
        public static IServiceCollection AddRegisterHangFire(this IServiceCollection services, IConfiguration configuration)
        {
            MySqlStorageOptions mySqlStorageOptions = new MySqlStorageOptions();
            configuration.GetSection("Hangfire:MySqlStorageOptions").Bind(mySqlStorageOptions);

            var storage = new MySqlStorage(
                    configuration.GetValue<string>("Hangfire:ConnectionStrings:hangfiredb"),
                    mySqlStorageOptions
                );            

            services.AddHangfire(
            (provider, config) => 
            {
                config
                    .UseStorage(storage)
                    .UseSerilogLogProvider()
                    .UseFilter(new HangFireLogStepsAttribute(provider.GetRequiredService<ILogger<HangFireLogStepsAttribute>>()));
            });
            return services;
        }

        /// <summary>Prove o uso do hangfire no serviço</summary>
        /// <param name="app">Application Builder</param>
        /// <param name="configuration">Configurações do sistema</param>
        public static IApplicationBuilder UseHangfireServer(this IApplicationBuilder app, IConfiguration configuration)
        {
            var backgroundServerOptions = new BackgroundJobServerOptions();

            configuration.Bind("Hangfire:BackgroundJobServerOptions", backgroundServerOptions);

            var users = configuration
                        .GetSection("Hangfire:DashboardOptions:BasicAuthAuthorizationFilterOptions:Users")
                        .GetChildren()
                        .Where(o => o.GetSection("Login").Exists() && o.GetSection("Password").Exists())
                        .Select(o => new BasicAuthAuthorizationUser() { Login = o.GetValue<string>("Login"), Password = Convert.FromBase64String(o.GetValue<string>("Password")) })
                        .ToArray();

            var basicAuth = new BasicAuthAuthorizationFilterOptions() { Users = users };
            configuration.GetSection("Hangfire:DashboardOptions:BasicAuthAuthorizationFilterOptions").Bind(basicAuth);



            return app
                .UseHangfireServer(backgroundServerOptions)
                .UseHangfireDashboard(
                "/hangfire",
                new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new BasicAuthAuthorizationFilter(basicAuth)
                    }
                });
        }
    }
}
