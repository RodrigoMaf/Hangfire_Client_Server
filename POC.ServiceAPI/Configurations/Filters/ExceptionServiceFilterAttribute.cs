using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace TVGlobo.GMusic.Phonogram.API.Configurations.Filters
{
    /// <summary>Filtro para tratar erros inesperados do sistema</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExceptionServiceFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>Default error message prefix</summary>
        private const string ERRORMESSAGE = "An error has occurred";

        /// <summary>Logger do sistema para o filtro</summary>
        private ILogger<ExceptionServiceFilterAttribute> Logger { get; }

        /// <summary>Inicia uma nova instância da classe <see cref="ExceptionServiceFilterAttribute" />.</summary>
        /// <param name="logger">Logger do sistema para o filtro</param>
        public ExceptionServiceFilterAttribute(ILogger<ExceptionServiceFilterAttribute> logger)
        {
            Logger = logger;
        }

        /// <summary>Trata o erro para o log e retorna no corpo a mensagem de erro</summary>
        /// <param name="context">Contexto de exception no sistema</param>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var ex = context.Exception;
            Logger.LogError(ex, ex.Message);            

            if (ex is ApplicationException)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 422,
                    Content = "Internal Server Error"
                };
            }
            else
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 500,
                    Content = "Internal server Error"
                };
            }

            return base.OnExceptionAsync(context);
        }
    }
}
