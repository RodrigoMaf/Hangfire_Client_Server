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
    /// <summary>Configurações de inicialização do serviço</summary>
    public class Startup
    {
        /// <summary>Provedor de configurações do serviço</summary>
        public IConfiguration Configuration { get; }

        /// <summary>Inicia uma nova instância da classe <see cref="Startup" />.</summary>
        /// <param name="configuration">Provedor de configurações do serviço</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>Configurações do serviço e injeção de dependencia</summary>
        /// <param name="services">Serviço de injeção de dependencia</param>        
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

        /// <summary>Configurações pós injeçõa de dependencia</summary>
        /// <param name="app">Provedor de builder da aplicação</param>
        /// <param name="env">Provedor de ambiente da aplicação</param>
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
