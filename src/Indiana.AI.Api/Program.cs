using System.IO;
using System.Reflection;
using Indiana.AI.Api.Security;
using Indiana.AI.Application;
using Indiana.AI.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Service registration
// ---------------------------------------------------------------------------
builder.Services.AddControllers();

// Clean Architecture layers.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Inbound security contract (X-API-Key / X-Source) — enforcement is opt-in.
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.SectionName));

// Health checks.
builder.Services.AddHealthChecks();

// Swagger / OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Indiana AI Document Verification Integration API",
        Version = "v1",
        Description = "POC that simulates the Indiana AI Document Verification integration platform: " +
                      "Upload API, Validate API, Artifact Retrieval API, payload mapping, tenant-aware routing, the " +
                      "adapter framework and verification-result persistence. The AI verification service " +
                      "and the tenant document stores are mocked."
    });

    // Document the AI Verification Service security contract (X-API-Key / X-Source).
    options.AddSecurityDefinition("X-API-Key", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "API key issued to the calling service."
    });
    options.AddSecurityDefinition("X-Source", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-Source",
        Description = "Identifier of the calling source system."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-API-Key" } }] = System.Array.Empty<string>(),
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-Source" } }] = System.Array.Empty<string>()
    });

    // Include XML documentation comments in Swagger.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// HTTP pipeline
// ---------------------------------------------------------------------------

// Swagger UI is available in every environment so the POC renders immediately.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Indiana AI Integration API v1");
    options.DocumentTitle = "Indiana AI Document Verification Integration";
    options.RoutePrefix = "swagger";
});

// Enforce the inbound security contract (no-op unless Security:RequireApiKey = true).
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Root redirects to Swagger UI.
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();

/// <summary>Program entry point exposed for integration testing.</summary>
public partial class Program { }
