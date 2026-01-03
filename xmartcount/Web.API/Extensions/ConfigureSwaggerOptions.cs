using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.API.Extensions;

public  class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
     private readonly IApiVersionDescriptionProvider _provider;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider )
    {
        _provider = provider;
    }
    public void Configure(SwaggerGenOptions options)
    {
       // Crea un documento Swagger por cada versión descubierta
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

      private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Smart Accounting API",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated ? "⚠️ Esta versión está DEPRECADA":"Documentación de la API con soporte para versionado.",
            Contact = new OpenApiContact { Name = "IT SUPPORT ", Email = "support@taxprosuite.com" }
        };     


     

    return info;

       
    }
}