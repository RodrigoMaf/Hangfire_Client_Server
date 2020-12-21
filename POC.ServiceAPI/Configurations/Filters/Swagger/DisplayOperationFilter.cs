using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace POC.ServiceAPI.Configurations.Filters.Swagger
{
    /// <summary>Inclue os dados no swagger de cada endpoint</summary>
    public class DisplayOperationFilter : IOperationFilter
    {
        /// <summary>Inclue descrições em cada endpoint do serviço</summary>
        /// <param name="operation">Operador de openapi</param>
        /// <param name="context">Operador de filtro de contexto</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var modelValidateAttributes = context.MethodInfo.DeclaringType
                                .GetCustomAttributes(true)
                                .Union(context.MethodInfo.GetCustomAttributes(true));

            ////if (HasMethodOK(context))
            ////{
            ////    operation.Responses.Add("200", new OpenApiResponse { Description = "Processou a requisição com sucesso." });
            ////}

            operation.Responses.Add("500", new OpenApiResponse { Description = "InternalServerError - Retorna uma mensagem de erro" });
                                   
            var actionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor == null)
            {
                return;
            }

            var displayAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (displayAttributes == null || displayAttributes.Length == 0)
            {
                return;
            }

            var displayAttribute = (DisplayAttribute)displayAttributes[0];

            operation.Description = displayAttribute.GetDescription();
        }

        /// <summary>Checa se o método tem um filtro do tipo indicado</summary>
        /// <typeparam name="T">Tipo do filtro</typeparam>
        /// <param name="modelValidateAttributes">Lista de filtros do método</param>
        private static bool HasFilterType<T>(IEnumerable<object> modelValidateAttributes)
            => modelValidateAttributes.OfType<T>().Any() ||
                modelValidateAttributes.Any(o => (o as TypeFilterAttribute)?.ImplementationType == typeof(T));

        /// <summary>Tem método ok</summary>
        /// <param name="context">Contexto de execução </param>
        private static bool HasMethodOK(OperationFilterContext context)
            => context.ApiDescription.HttpMethod == "GET" || context.ApiDescription.HttpMethod == "DELETE";

        /// <summary>Configura o escopo de tratamento para autenticação</summary>
        /// <param name="operation">Operador de openapi</param>
        /// <param name="context">Operador de filtro de contexto</param>
        private static void MapAuthenticationScope(OpenApiOperation operation, OperationFilterContext context) 
        {
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var authScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [authScheme] = requiredScopes.ToList()
                    }
                };
            }
        }
    }
}
