using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace POC.ServiceAPI.Configurations.Middlewares
{
    /// <summary>Busca e loga os dados de reference da requisição recebida</summary>
    public class ReferenceMiddleware
    {
        /// <summary>Request delegate function</summary>
        private RequestDelegate Next { get; }

        /// <summary>Inicia uma nova instância da classe <see cref="ReferenceMiddleware" />.</summary>
        /// <param name="next">Delegate next process pipeline</param>
        public ReferenceMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        /// <summary>Invoke middleware</summary>
        /// <param name="context">Http request context</param>
        public async Task Invoke(HttpContext context)
        {            
            StringValues traceId = string.Empty, xOriginalFor = string.Empty;
            context.Request.Headers.TryGetValue("X-Original-For", out xOriginalFor);
            if (context.Request.Headers.TryGetValue("TraceId", out traceId) == false)
            {
                traceId = Guid.NewGuid().ToString();
                context.Request.Headers.Add("TraceId", traceId);
            }

            using (LogContext.PushProperty("TraceId", traceId.ToString()))
            {
                context
                    .Response
                    .OnStarting(
                        state => 
                        {
                            var httpContext = (HttpContext)state;

                            httpContext.Response.Headers.Add("TraceId", traceId.ToString());
                            var keysName = string.Join(",", httpContext.Response.Headers.Keys);
                            httpContext.Response.Headers.Add("Access-Control-Expose-Headers", keysName);
                            return Task.CompletedTask;
                        }, 
                        context
                    );

                await Next(context);
            }
        }
    }
}
