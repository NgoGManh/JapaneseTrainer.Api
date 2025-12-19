using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace JapaneseTrainer.Api.Swagger
{
    /// <summary>
    /// Swagger operation filter to add default values to query parameters from complex objects
    /// </summary>
    public class SwaggerDefaultValueOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            // Find [FromQuery] complex object parameters
            var methodParameters = context.MethodInfo.GetParameters();
            foreach (var methodParam in methodParameters)
            {
                var fromQueryAttr = methodParam.GetCustomAttribute<FromQueryAttribute>();
                if (fromQueryAttr != null && methodParam.ParameterType.IsClass && methodParam.ParameterType != typeof(string))
                {
                    // This is a complex object used as [FromQuery]
                    var properties = methodParam.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    
                    foreach (var property in properties)
                    {
                        var fromQueryNameAttr = property.GetCustomAttribute<FromQueryAttribute>();
                        // Use the Name from [FromQuery(Name = "...")] attribute, or convert property name to snake_case
                        var paramName = fromQueryNameAttr?.Name ?? ConvertPascalToSnakeCase(property.Name);
                        
                        // Find the corresponding OpenAPI parameter (check exact match and case-insensitive)
                        var openApiParam = operation.Parameters.FirstOrDefault(p => 
                            p.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase) ||
                            p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                        
                        if (openApiParam != null)
                        {
                            var defaultValueAttr = property.GetCustomAttribute<DefaultValueAttribute>();
                            if (defaultValueAttr != null)
                            {
                                // Set default value
                                var defaultValue = defaultValueAttr.Value;
                                if (defaultValue != null)
                                {
                                    if (openApiParam.Schema != null)
                                    {
                                        openApiParam.Schema.Default = CreateOpenApiAny(defaultValue);
                                    }
                                    
                                    // Update description to show default
                                    var defaultText = $"Default: {defaultValue}";
                                    if (!string.IsNullOrEmpty(openApiParam.Description))
                                    {
                                        if (!openApiParam.Description.Contains("Default:", StringComparison.OrdinalIgnoreCase))
                                        {
                                            openApiParam.Description = $"{openApiParam.Description.TrimEnd('.')}. {defaultText}.";
                                        }
                                    }
                                    else
                                    {
                                        openApiParam.Description = defaultText;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ConvertPascalToSnakeCase(string pascalCase)
        {
            if (string.IsNullOrEmpty(pascalCase)) return pascalCase;
            
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < pascalCase.Length; i++)
            {
                if (i > 0 && char.IsUpper(pascalCase[i]))
                {
                    result.Append('_');
                }
                result.Append(char.ToLowerInvariant(pascalCase[i]));
            }
            return result.ToString();
        }

        private Microsoft.OpenApi.Any.IOpenApiAny CreateOpenApiAny(object? value)
        {
            if (value == null)
                return new Microsoft.OpenApi.Any.OpenApiNull();
            
            if (value is int intValue)
                return new Microsoft.OpenApi.Any.OpenApiInteger(intValue);
            
            if (value is string strValue)
                return new Microsoft.OpenApi.Any.OpenApiString(strValue);
            
            if (value is bool boolValue)
                return new Microsoft.OpenApi.Any.OpenApiBoolean(boolValue);
            
            return new Microsoft.OpenApi.Any.OpenApiString(value.ToString() ?? "");
        }
    }
}

