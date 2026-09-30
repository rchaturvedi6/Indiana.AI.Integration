using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Infrastructure.Adapters;
using Indiana.AI.Infrastructure.Artifacts;
using Indiana.AI.Infrastructure.Configuration;
using Indiana.AI.Infrastructure.Fms;
using Indiana.AI.Infrastructure.Live;
using Indiana.AI.Infrastructure.Persistence;
using Indiana.AI.Infrastructure.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Indiana.AI.Infrastructure;

/// <summary>
/// Dependency injection registration for the Infrastructure layer. Chooses
/// between the in-process mocks and the live Enso / FMS clients based on the
/// <c>Integration:UseMocks</c> configuration flag, and wires tenant-aware
/// routing from the <c>Routing</c> configuration section.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers Infrastructure services using the supplied configuration.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new IntegrationOptions();
        configuration.GetSection(IntegrationOptions.SectionName).Bind(options);
        services.Configure<IntegrationOptions>(configuration.GetSection(IntegrationOptions.SectionName));

        // Tenant-aware routing configuration (Routing section).
        services.Configure<RoutingOptions>(configuration.GetSection(RoutingOptions.SectionName));

        AddSharedServices(services);

        return options.UseMocks
            ? services.AddMockIntegration()
            : services.AddLiveIntegration(options);
    }

    /// <summary>
    /// Convenience overload that always registers the mock integration with a
    /// default Indiana→IndianaOracleAdapter routing map (used by tests and when
    /// no configuration is available).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.Configure<RoutingOptions>(o =>
        {
            o.DefaultTenant = "Indiana";
            o.Tenants["Indiana"] = "IndianaOracleAdapter";
            o.Tenants["Nevada"] = "SqlAdapter";
            o.Tenants["FutureState"] = "MockFileAdapter";
        });

        AddSharedServices(services);
        return services.AddMockIntegration();
    }

    private static void AddSharedServices(IServiceCollection services)
    {
        // Used by both mock and live modes.
        services.AddSingleton<IVerificationResultRepository, InMemoryVerificationResultRepository>();

        // Tenant-aware routing + adapter framework.
        services.AddSingleton<ITenantConfigurationProvider, TenantConfigurationProvider>();
        services.AddSingleton<IAdapterResolver, AdapterResolver>();
        services.AddScoped<IArtifactRetrievalService, ArtifactRetrievalService>();

        // Mock backing store (seeded documents), shared by the mock adapters and
        // by the stand-in File Management Server endpoint.
        services.AddSingleton<MockEdmsRepository>();

        // Stand-in File Management Server (GetEDMSFileQuery). Always mock — the AI
        // Verification Service calls this to download a file during validation.
        services.AddSingleton<IFileManagementService, MockFileManagementService>();

        // The AI Team's verification boundary. In the POC this is always the mock:
        // in mock mode the LocalAiPlatformClient drives it; in live mode Enso performs
        // validation and this registration simply keeps the DI graph valid (the
        // ValidationEngine is constructed-but-unused because EnsoAiPlatformClient
        // overrides IAiPlatformClient).
        services.AddSingleton<MockExtractionDataset>();
        services.AddSingleton<IDocumentVerificationService, MockDocumentVerificationService>();
    }

    /// <summary>Registers the in-process mock adapters and mock AI verification.</summary>
    private static IServiceCollection AddMockIntegration(this IServiceCollection services)
    {
        // Tenant adapters behind a common contract (Indiana→Oracle, Nevada→Sql, FutureState→File).
        services.AddSingleton<IDocumentAdapter, IndianaOracleAdapter>();
        services.AddSingleton<IDocumentAdapter, SqlAdapter>();
        services.AddSingleton<IDocumentAdapter, MockFileAdapter>();

        return services;
    }

    /// <summary>
    /// Registers the live Enso AI platform client (replacing the default local
    /// mock) and the real FMS adapter, both configured with typed HttpClients.
    /// </summary>
    private static IServiceCollection AddLiveIntegration(this IServiceCollection services, IntegrationOptions options)
    {
        var enso = options.Enso;
        var fms = options.Fms;

        // Live Enso client (API 1 + API 3): overrides the default LocalAiPlatformClient.
        services.AddHttpClient(nameof(EnsoAiPlatformClient), client =>
        {
            client.BaseAddress = new Uri(enso.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(enso.ApiKey))
            {
                client.DefaultRequestHeaders.Add("X-API-Key", enso.ApiKey);
            }
            if (!string.IsNullOrWhiteSpace(enso.Source))
            {
                client.DefaultRequestHeaders.Add("X-Source", enso.Source);
            }
        });
        services.AddScoped<IAiPlatformClient>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(EnsoAiPlatformClient));
            return new EnsoAiPlatformClient(http, enso.UploadPath, enso.ValidatePath);
        });

        // Live Indiana adapter (API 2) — real FMS/EDMS GetEDMSFileQuery.
        services.AddHttpClient(nameof(RealFmsAdapter), client =>
        {
            client.BaseAddress = new Uri(fms.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddSingleton<IDocumentAdapter>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(RealFmsAdapter));
            return new RealFmsAdapter(http, fms.GetEdmsFilePath);
        });

        return services;
    }
}
