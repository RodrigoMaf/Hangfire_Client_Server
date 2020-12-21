using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace POC.ServiceAPI.Configuration.Configurations.Filters.Swagger
{
    /// <summary>Classe para formatação do enum</summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>Aplica formatação de exibição do enum no schema</summary>
        /// <param name="schema">Schema do swagger</param>
        /// <param name="context">Contexto de montagem</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var dados = context?.MemberInfo?.GetInlineAndMetadataAttributes()?.FirstOrDefault() as DisplayAttribute;
            if (dados != null) 
            {
                schema.Description = dados.GetDescription();
                schema.Title = dados.GetName();
            }            

            if (context.Type.IsEnum)
            {
                var enumValues = schema.Enum.ToArray();
                var i = 0;
                schema.Enum.Clear();
                foreach (var n in Enum.GetNames(context.Type).ToList())
                {
                    schema.Enum.Add((OpenApiPrimitive<int>)enumValues[i]);
                    schema.Enum.Add(new OpenApiString(n));                    
                    i++;
                }
            }
        }
    }
}