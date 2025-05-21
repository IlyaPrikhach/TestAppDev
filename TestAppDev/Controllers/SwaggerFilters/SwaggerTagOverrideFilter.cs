using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace TestAppDev.Controllers.SwaggerFilters;

public class SwaggerTagOverrideFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
        var controllerType = controllerDescriptor?.ControllerTypeInfo;

        var swaggerTag = controllerType?.GetCustomAttribute<SwaggerTagAttribute>();
        var tagName = swaggerTag?.Description ?? controllerDescriptor?.ControllerName;

        if (!string.IsNullOrEmpty(tagName))
        {
            operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = tagName } };
        }
    }
}