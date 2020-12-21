using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TVGlobo.GMusic.Phonogram.API.Configurations.Filters;

namespace POC.ServiceAPI.Configurations.Registers.Mvc
{
    /// <summary>Configurações de Opções do MVC por injeção de dependencia</summary>
    internal class MvcOptionsConfigure : IConfigureOptions<MvcOptions>
    {
        #region Properties

        /// <summary>Configuration settings</summary>
        private IConfiguration Configuration { get; }

        #endregion

        /// <summary>Inicia uma nova instância da classe <see cref="MvcOptionsConfigure" />.</summary>
        /// <param name="configuration">Configuration settings</param>
        /// <param name="csvOutputFormatter">Formatador de saida CSV</param>
        /// <param name="csvInputFormatter">Formatador de entrada CSV</param>
        public MvcOptionsConfigure(
                                      IConfiguration configuration
                                  )
        {
            Configuration = configuration;
        }

        /// <summary>Configura as opções de mvc</summary>
        /// <param name="options">Opções de configuração de mvc</param>
        public void Configure(MvcOptions options)
        {
            Configuration.GetSection("MvcOptions").Bind(options);
            options.Filters.Add<ExceptionServiceFilterAttribute>();
        }
    }
}
