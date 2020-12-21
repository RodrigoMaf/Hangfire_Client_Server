using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace POC.ServiceAPI.Configurations.Registers
{
    /// <summary>Registra convenções de api</summary>
    public static class RegisterVersionConventionsExtensions
    {
        /// <summary>Faz o registro de metricas no serviço DI</summary>
        /// <param name="services">Serviço DI</param>
        /// <param name="configuration">Configurações do sistema</param>
        public static IServiceCollection AddVersionConventions(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddApiVersioning(
                o =>
                {
                    o.ReportApiVersions = true;
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.ApiVersionReader = new UrlSegmentApiVersionReader();
                    ////ApiVersionReader.Combine(
                    ////   new QueryStringApiVersionReader("api-version"),
                    ////   new HeaderApiVersionReader("api-version"),
                    ////   new UrlSegmentApiVersionReader()
                    ////);

                    var v1Types = Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => string.Equals(t.Namespace, "POC.ServiceAPI.Controllers.V1", StringComparison.Ordinal));

                    foreach (var type in v1Types)
                    {
                        o.Conventions.Controller(type).HasApiVersion(new ApiVersion(1, 0));
                    }
                });
    }
}
