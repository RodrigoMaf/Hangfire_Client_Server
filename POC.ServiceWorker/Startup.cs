using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.Application;
using POC.Infra;
using POC.Infra.Registers;
using POC.ServiceWorker.Configurations;
using POC.ServiceWorker.Configurations.Registers;

namespace POC.ServiceWorker
{
    /// <summary>Configura��es de inicializa��o do servi�o</summary>
    public class Startup
    {
        /// <summary>Provedor de configura��es do servi�o</summary>
        public IConfiguration Configuration { get; }

        /// <summary>Inicia uma nova inst�ncia da classe <see cref="Startup" />.</summary>
        /// <param name="configuration">Provedor de configura��es do servi�o</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>Configura��es do servi�o e inje��o de dependencia</summary>
        /// <param name="services">Servi�o de inje��o de dependencia</param>        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddWebApi()
                .AddSecurityFeatures(Configuration)
                .RegisterApplicationServices(Configuration)
                .RegisterDataBase(Configuration)
                .AddRegisterHangFire(Configuration)
                ;
        }

        /// <summary>Configura��es p�s inje��a de dependencia</summary>
        /// <param name="app">Provedor de builder da aplica��o</param>
        /// <param name="env">Provedor de ambiente da aplica��o</param>
        public void Configure(
                                IApplicationBuilder app, 
                                IWebHostEnvironment env
                             )
        {
            app.UseDeveloperExceptionPage();
            app
                //.UseSecurityFeatures(env)
                .UseRoutingConfig()
                .UseHangfireServer(Configuration);
        }
    }
}
