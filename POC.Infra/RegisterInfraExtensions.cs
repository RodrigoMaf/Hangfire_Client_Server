using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.Domain.Infra;
using POC.Infra.RepositoryContext;

namespace POC.Infra
{
    /// <summary>Registra features de infra no sistema</summary>
    public static class RegisterInfraExtensions
    {
        /// <summary>Registro da camada infra</summary>
        /// <param name="services">Serviço de injeção de dependencia</param>
        /// <param name="configuration">Provedor de configuração do serviço</param>
        public static IServiceCollection RegisterInfraServices(
                                                                this IServiceCollection services,
                                                                IConfiguration configuration
                                                              )
            => services
                .AddSingleton<IFeatureInfra, FeatureInfra>();

        public static IServiceCollection RegisterDataBase(
                                                            this IServiceCollection services,
                                                            IConfiguration configuration
                                                         )
        {
            services
                .AddDbContext<DbContext>(options =>
                    options.UseMySQL(configuration.GetValue<string>("Hangfire:ConnectionStrings:hangfiredb"))
                )
                .AddScoped<IDbSetHangfireContext, DbSetHangfireContext>();

            services.BuildServiceProvider().GetService<IDbSetHangfireContext>().CreateDatabase();

            return services;
        }
    }
}
