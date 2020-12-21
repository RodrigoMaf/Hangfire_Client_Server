using POC.ServiceWorker.Configurations.Registers.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace POC.ServiceWorker.Configurations
{
    /// <summary>Classe de configuração de roteamento da api</summary>
    public static class WebApiServiceCollectionExtensions
    {
        /// <summary>Configura uma api default no sistema</summary>
        /// <param name="services">Provedor de configuração de DI para o serviço</param>
        public static IServiceCollection AddWebApi(
                                                     this IServiceCollection services
                                                  )
        {
            services
                .AddSingleton<IConfigureOptions<CorsOptions>, CorsOptionsConfigure>();

            services
                .AddCors();
                ////.AddMvcCore()
                ////.AddApiExplorer()
                ////.AddDataAnnotations()
                ////.AddJsonOptions(options => 
                ////{
                ////    var opt = options.JsonSerializerOptions;
                ////    opt.IgnoreReadOnlyProperties = true;
                ////    opt.IgnoreNullValues = true;
                ////});

            return services;
        }

        /// <summary>Configura o roteamento do WebApi</summary>
        /// <param name="app">Provedor de builder da aplicação</param>
        public static IApplicationBuilder UseRoutingConfig(this IApplicationBuilder app)
            => app
                   .UseRouting()
                   .UseCors(CorsOptionsConfigure.MainPolicyName);
    }
}