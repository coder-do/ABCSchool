using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.OpenApi
{
    public class SwaggerHeaderAttributeProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (context.MethodInfo.GetCustomAttribute(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute attribute)
            {
                var parameters = context.OperationDescription.Operation.Parameters;

                var existingParam = parameters
                    .FirstOrDefault(p => p.Kind == OpenApiParameterKind.Header && p.Name == attribute.HeaderName);

                if (existingParam != null)
                {
                    parameters.Remove(existingParam);
                }

                parameters.Add(new OpenApiParameter
                {
                    Name = attribute.HeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Description = attribute.Description,
                    IsRequired = attribute.IsRequired,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        Default = attribute.DefaultValue,
                    }
                });
            }

            return true;
        }
    }
}
