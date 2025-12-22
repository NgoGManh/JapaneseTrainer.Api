using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace JapaneseTrainer.Api.Swagger
{
    /// <summary>
    /// Swagger operation filter to properly display file upload in Swagger UI
    /// </summary>
    public class SwaggerFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile) ||
                           (p.ParameterType.IsClass && 
                            p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(Microsoft.AspNetCore.Http.IFormFile))))
                .ToList();

            if (!fileParameters.Any())
                return;

            // Remove existing parameters that are file-related
            operation.Parameters?.Clear();
            operation.RequestBody = null;

            // Create request body for file upload
            var properties = new Dictionary<string, OpenApiSchema>();
            
            foreach (var param in fileParameters)
            {
                if (param.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile))
                {
                    // Single file parameter
                    properties[param.Name ?? "file"] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary",
                        Description = "Excel file to upload"
                    };
                }
                else
                {
                    // Complex type with IFormFile property
                    var fileProperty = param.ParameterType.GetProperties()
                        .FirstOrDefault(p => p.PropertyType == typeof(Microsoft.AspNetCore.Http.IFormFile));
                    
                    if (fileProperty != null)
                    {
                        properties[fileProperty.Name] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary",
                            Description = "Excel file to upload"
                        };
                    }
                }
            }

            if (properties.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = properties,
                                Required = new HashSet<string>(properties.Keys)
                            }
                        }
                    },
                    Required = true
                };
            }
        }
    }
}


