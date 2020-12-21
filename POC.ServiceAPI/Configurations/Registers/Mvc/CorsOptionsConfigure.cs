using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace POC.ServiceAPI.Configurations.Registers.Mvc
{
    /// <summary>Configuração de cors no sistema</summary>
    public class CorsOptionsConfigure : IConfigureOptions<CorsOptions>
    {
        /// <summary>Nome da politica de acesso ao serviço</summary>
        public static string MainPolicyName { get => "mainCorsPolice"; }

        /// <summary>Provedor de configurações do serviço</summary>
        private IConfiguration Configuration { get; }

        /// <summary>Inicia uma nova instância da classe <see cref="CorsOptionsConfigure" />.</summary>
        /// <param name="configuration">Provedor de configurações do serviço</param>
        public CorsOptionsConfigure(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        /// <summary>Configura os dados de Cors no sistema</summary>
        /// <param name="options">Opções de configuração de cors</param>
        public void Configure(CorsOptions options)
        {
            if (Configuration.GetSection($"Security:Cors:{MainPolicyName}").Exists() == false) 
            {
                throw new ArgumentException($"Necessita existir uma configuração principal chamada {MainPolicyName}", MainPolicyName);
            }                

            foreach (var item in Configuration.GetSection("Security:Cors").GetChildren())
            {
                options.AddPolicy(
                item.Key,
                o =>
                {
                    SetAllowedOrigins(o, item);
                    SetAllowedHeaders(o, item);
                    SetAllowedMethods(o, item);                    
                    o.Build();
                });
            }          
        }

        /// <summary>Set Allowed Origins</summary>
        /// <param name="builder">Cors policy builder configuration</param>
        /// <param name="itemCorsSection">Item configuration</param>
        private void SetAllowedOrigins(CorsPolicyBuilder builder, IConfiguration itemCorsSection) 
        {
            var origins = itemCorsSection.GetSection("origins").Get<string[]>();
            if (origins != null)
            {
                builder.WithOrigins(origins);
            }
            else
            {
                builder.AllowAnyOrigin();
            }
        }

        /// <summary>Set Allowed Headers</summary>
        /// <param name="builder">Cors policy builder configuration</param>
        /// <param name="itemCorsSection">Item configuration</param>
        private void SetAllowedHeaders(CorsPolicyBuilder builder, IConfiguration itemCorsSection)
        {
            var headers = itemCorsSection.GetSection("headers").Get<string[]>();
            if (headers != null)
            {
                builder.WithHeaders(headers);
            }
            else
            {
                builder.AllowAnyHeader();
            }
        }

        /// <summary>Set Allowed Methods</summary>
        /// <param name="builder">Cors policy builder configuration</param>
        /// <param name="itemCorsSection">Item configuration</param>
        private void SetAllowedMethods(CorsPolicyBuilder builder, IConfiguration itemCorsSection)
        {
            var methods = itemCorsSection.GetSection("methods").Get<string[]>();
            if (methods != null)
            {
                builder.WithMethods(methods);
            }
            else
            {
                builder.AllowAnyMethod();
            }
        }
    }
}
