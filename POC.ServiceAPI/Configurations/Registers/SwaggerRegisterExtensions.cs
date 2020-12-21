using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using POC.ServiceAPI.Configuration.Configurations.Filters.Swagger;
using POC.ServiceAPI.Configurations.Filters.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace POC.ServiceAPI.Configurations
{
    /// <summary>Classe de extensão para configuração do swagger</summary>
    public static class SwaggerRegisterExtensions
    {
        /// <summary>String default de tags</summary>
        private static readonly string[] DefaultSwaggerTags = new[] { "string" };

        /// <summary>Registra uma configuração para o funcionamento do swagger no serviço</summary>
        /// <param name="services">Provedor de configuração de DI para o serviço</param>
        /// <param name="configuration">Provedor de dados de configurações do serviço</param>
        public static IServiceCollection RegisterSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerInfos = configuration
                                .GetSection("Swagger:Infos")
                                .GetChildren()
                                .Select(o =>
                                {
                                    var apiInfo = new OpenApiInfo();
                                    o.Bind(apiInfo);
                                    return apiInfo;
                                }).ToArray();

            var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "POC.ServiceAPI.xml");

            services
                .AddSwaggerGen(options =>
                {
                    foreach (var item in swaggerInfos)
                    {
                        options.SwaggerDoc(item.Version, item);
                    }

                    ////AddSecurity(options);

                    //// this will group actions by localized name set in controller's DisplayAttribute
                    ////options.TagActionsBy(GetSwaggerTags);
                    ////Ordenar lista por ...
                    ////options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

                    options.DescribeAllParametersInCamelCase();
                    options.SchemaFilter<EnumSchemaFilter>();

                    ////options.IncludeXmlComments(xmlPath);
                    //// this will add localized description to actions set in action's DisplayAttribute
                    options.OperationFilter<DisplayOperationFilter>();
                });

            return services;
        }        

        /// <summary>Configuração do funcionamento do swagger</summary>
        /// <param name="app">Provedor de configuração da aplicação</param>
        /// <param name="openApiSettings">Dados de swagger configurado na injeção de dependencia</param>
        public static IApplicationBuilder UseSwagger(
                                                this IApplicationBuilder app,
                                                IOptionsMonitor<List<OpenApiInfo>> openApiSettings
                                           )
        {
            var openApiData = openApiSettings.CurrentValue;
            return app
                .UseSwagger(s =>
                {
                    s.SerializeAsV2 = true;
                    s.RouteTemplate = $"{openApiData.FirstOrDefault().Title}/swagger/{{documentName}}/swagger.json";
                })
                .UseSwaggerUI(s =>
                {
                    ////s.OAuthClientId("test-id");
                    ////s.OAuthClientSecret("test-secret");
                    ////s.OAuthRealm("test-realm");
                    ////s.OAuthAppName("test-app");
                    ////s.OAuthScopeSeparator(" ");
                    ////s.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" } });
                    ////s.OAuthUseBasicAuthenticationWithAccessCodeGrant();

                    ////s.DefaultModelExpandDepth(2);
                    ////s.DefaultModelRendering(ModelRendering.Model);
                    ////s.DefaultModelsExpandDepth(-1);
                    ////s.DisplayOperationId();
                    ////s.DisplayRequestDuration();
                    ////s.DocExpansion(DocExpansion.None);
                    ////s.EnableDeepLinking();
                    ////s.EnableFilter();
                    ////s.MaxDisplayedTags(5);
                    ////s.ShowExtensions();
                    ////s.ShowCommonExtensions();
                    ////s.EnableValidator();
                    ////s.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head);

                    foreach (var item in openApiData)
                    {
                        s.SwaggerEndpoint($"/{item.Title}/swagger/{item.Version}/swagger.json", item.Description);
                    }
                                        
                    s.RoutePrefix = "swagger";
                });
        }

        /// <summary>Obtem as decrições das apis de acordo com o display</summary>
        /// <param name="description">Descrição de apis</param>
        private static string[] GetSwaggerTags(ApiDescription description)
        {
            var actionDescriptor = description.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor == null)
            {
                return DefaultSwaggerTags;
            }

            var displayAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (displayAttributes == null || displayAttributes.Length == 0)
            {
                return new[]
                {
                    actionDescriptor.ControllerName
                };
            }

            var displayAttribute = (DisplayAttribute)displayAttributes[0];

            return new[]
            {
                displayAttribute.GetDescription()
            };
        }

        /// <summary>Adiciona autenticação no serviço</summary>
        /// <param name="options">Opção de configuração</param>
        private static void AddSecurity(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(
            "oauth2", 
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("/auth-server/connect/authorize", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            { "readAccess", "Access read operations" },
                            { "writeAccess", "Access write operations" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[] { "readAccess", "writeAccess" }
                }
            });
        }
    }
}
