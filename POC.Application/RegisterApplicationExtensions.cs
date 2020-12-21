using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.Domain.Application;
using POC.Infra;

namespace POC.Application
{
    /// <summary>Registra features de application no sistema</summary>
    public static class RegisterApplicationExtensions
    {
        /// <summary>Registro da camada de application</summary>
        /// <param name="services">Serviço de injeção de dependencia</param>
        /// <param name="configuration">Provedor de configuração do serviço</param>
        public static IServiceCollection RegisterApplicationServices(
                                                                this IServiceCollection services,
                                                                IConfiguration configuration
                                                              )
            => services
                  .AddSingleton<IFeatureApplication, FeatureApplication>()
                  .RegisterInfraServices(configuration);

    }
}
