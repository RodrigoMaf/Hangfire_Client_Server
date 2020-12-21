using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace POC.Infra.RepositoryContext
{
    /// <summary>Configuração do banco do hangfire</summary>
    public interface IDbSetHangfireContext 
    {
        /// <summary>Cria o banco de dado</summary>
        bool CreateDatabase();
    }

    /// <summary>Implementação da feature <see cref="IDbCreate"/></summary>
    internal class DbSetHangfireContext : DbContext, IDbSetHangfireContext
    {
        #region Properties

        /// <summary>Log Factory</summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>Provedor de configuração</summary>
        private IConfiguration Configuration { get; }

        #endregion

        /// <summary>Inicia uma nova instância da classe <see cref="DbSetHangfireContext" />.</summary>
        /// <param name="options">Opções de configuração do banco de dados</param>
        /// <param name="loggerFactory">Log Factory</param>
        /// <param name="configuration">Provedor de configuração</param>
        public DbSetHangfireContext(
                                DbContextOptions<DbContext> options,
                                ILoggerFactory loggerFactory,
                                IConfiguration configuration
                           ) : base(options)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        /// <summary>Cria o banco de dado</summary>
        public bool CreateDatabase()
        {
            try
            {
                Database.EnsureDeleted();
            }
            catch (System.Exception)
            {

                throw;
            }
            

            return Database.EnsureCreated();
        }

        /// <summary>Evento de configuração do banco de dados</summary>
        /// <param name="optionsBuilder">Opções da configuração do banco de dados</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            optionsBuilder
                ////.UseMySQL(Configuration.GetValue<string>("ConnectionStrings:hangfiredb"))
                .UseLoggerFactory(LoggerFactory)
                .EnableSensitiveDataLogging(true)
                .EnableDetailedErrors(true)
                .ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug)));

            base.OnConfiguring(optionsBuilder);
        }
    }
}
