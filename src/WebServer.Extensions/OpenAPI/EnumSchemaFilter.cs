using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AV.Household.WebServer.Extensions.OpenAPI;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        model.Enum.Clear();

        Enum.GetNames(context.Type).ToList()
            .ForEach(name => model.Enum.Add(new OpenApiString(name)));
    }
}
