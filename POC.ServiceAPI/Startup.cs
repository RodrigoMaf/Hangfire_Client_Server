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
    /// <summary>Configura��es de inicializa��o do servi�o</summary>
    public class Startup
    {
        #region Properties

        /// <summary>Provedor de configura��es do servi�o</summary>
        private IConfiguration Configuration { get; }

        /// <summary>Provedor de configura��es do servi�o</summary>
        private IWebHostEnvironment Environment { get; }

        #endregion

        /// <summary>Inicia uma nova inst�ncia da classe <see cref="Startup" />.</summary>
        /// <param name="configuration">Provedor de configura��es do servi�o</param>
        /// <param name="env">Provedor de ambiente da aplica��o</param>
        public Startup(
                          IConfiguration configuration,
                          IWebHostEnvironment env
                      )
        {
            Configuration = configuration;
            Environment = env;
        }

        /// <summary>Configura��es do servi�o e inje��o de dependencia</summary>
        /// <param name="services">Servi�o de inje��o de dependencia</param>        
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

        /// <summary>Configura��es p�s inje��a de dependencia</summary>
        /// <param name="app">Provedor de builder da aplica��o</param>    
        /// <param name="optionsMonitor">Dados da configura��o do swagger</param>
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
