using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace POC.ServiceAPI.Configurations
{
    /// <summary>Realiza o bind das parametrizações do serviço para as classes de configurações</summary>
    public static class ConfigureSettingsDataExtensions
    {
        /// <summary>Configurações vindas do arquivo para classe</summary>
        /// <param name="services">Provedor de Injeção de dependencia</param>
        /// <param name="configuration">Provedor de configuração do serviço</param>
        public static IServiceCollection ConfigureSettings(
                                                             this IServiceCollection services, 
                                                             IConfiguration configuration
                                                          ) 
        {            
            services
                .AddOptions<List<OpenApiInfo>>()
                .Configure(options => configuration.GetSection("Swagger:Infos").Bind(options))
                .PostConfigure(o => { });

            return services;
        }
    }
}
