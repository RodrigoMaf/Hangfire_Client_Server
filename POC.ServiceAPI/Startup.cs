using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using POC.Application;
using POC.Infra.Registers;
using POC.ServiceAPI.Configurations;
using POC.ServiceAPI.Configurations.Middlewares;
using POC.ServiceAPI.Configurations.Registers;

namespace POC.ServiceAPI
{
    /// <summary>Configurações de inicialização do serviço</summary>
    public class Startup
    {
        #region Properties

        /// <summary>Provedor de configurações do serviço</summary>
        private IConfiguration Configuration { get; }

        /// <summary>Provedor de configurações do serviço</summary>
        private IWebHostEnvironment Environment { get; }

        #endregion

        /// <summary>Inicia uma nova instância da classe <see cref="Startup" />.</summary>
        /// <param name="configuration">Provedor de configurações do serviço</param>
        /// <param name="env">Provedor de ambiente da aplicação</param>
        public Startup(
                          IConfiguration configuration,
                          IWebHostEnvironment env
                      )
        {
            Configuration = configuration;
            Environment = env;
        }

        /// <summary>Configurações do serviço e injeção de dependencia</summary>
        /// <param name="services">Serviço de injeção de dependencia</param>        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureSettings(Configuration)
                .RegisterApplicationServices(Configuration)
                .RegisterSwagger(Configuration)
                ////.RegisterDataBase(Configuration)
                .AddRegisterHangFire(Configuration)
                .AddWebApi()
                .AddSecurityFeatures(Configuration)
                ;
        }

        /// <summary>Configurações pós injeçõa de dependencia</summary>
        /// <param name="app">Provedor de builder da aplicação</param>    
        /// <param name="optionsMonitor">Dados da configuração do swagger</param>
        public void Configure(
                                IApplicationBuilder app,
                                IOptionsMonitor<List<OpenApiInfo>> optionsMonitor
                             )
        {
            app
                .UseDeveloperExceptionPage()
                .UseSecurityFeatures(Environment)
                .UseMiddleware<ReferenceMiddleware>()
                .UseSwagger(optionsMonitor)
                .UseRoutingConfig()
                ////.UseHangfireServer(Configuration);
            ;
        }
    }
}
