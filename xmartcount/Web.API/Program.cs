using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;

        // versionado por URL segment: api/v{version}/...
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";   // v1, v2
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Web.API.xml"));

    // Incluye endpoints solo en su doc (v1/v2)
    c.DocInclusionPredicate((docName, apiDesc) =>
        string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase));

    c.CustomSchemaIds(t => t.FullName);
});

builder.Services.ConfigureOptions<Web.API.Extensions.ConfigureSwaggerOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }

        await next();
    });
}

// DEBUG: ver versiones detectadas
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    Console.WriteLine("== API Versions discovered ==");
    foreach (var d in provider.ApiVersionDescriptions)
        Console.WriteLine($"Group={d.GroupName} Version={d.ApiVersion} Deprecated={d.IsDeprecated}");
}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        // ✅ CLAVE: usar ./ para resolver bien desde el UI
        c.SwaggerEndpoint(
            $"./swagger/{description.GroupName}/swagger.json",
            $"Smart Accounting API {description.GroupName.ToUpperInvariant()}");
    }

    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
