using System.Collections.Generic;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Application.Mapping;
using Indiana.AI.Application.Services;
using Indiana.AI.Infrastructure.Adapters;
using Indiana.AI.Infrastructure.Artifacts;
using Indiana.AI.Infrastructure.Configuration;
using Indiana.AI.Infrastructure.Persistence;
using Indiana.AI.Infrastructure.Routing;
using Microsoft.Extensions.Options;

namespace Indiana.AI.Tests;

/// <summary>Shared builders for wiring the tenant-aware mock stack in tests.</summary>
internal static class TestFactory
{
    public static ITenantConfigurationProvider TenantProvider(string? defaultTenant = "Indiana")
    {
        var options = new RoutingOptions
        {
            DefaultTenant = defaultTenant,
            Tenants = new Dictionary<string, string>
            {
                ["Indiana"] = "IndianaOracleAdapter",
                ["Nevada"] = "SqlAdapter",
                ["FutureState"] = "MockFileAdapter"
            }
        };
        return new TenantConfigurationProvider(Options.Create(options));
    }

    public static AdapterResolver Resolver(MockEdmsRepository repo)
    {
        var adapters = new List<IDocumentAdapter>
        {
            new IndianaOracleAdapter(repo),
            new SqlAdapter(repo),
            new MockFileAdapter(repo)
        };
        return new AdapterResolver(adapters);
    }

    public static Router Router(MockEdmsRepository repo, string? defaultTenant = "Indiana") =>
        new(TenantProvider(defaultTenant), Resolver(repo));

    public static ArtifactRetrievalService ArtifactRetrieval(MockEdmsRepository repo) =>
        new(Router(repo));

    public static ValidationEngine ValidationEngine()
    {
        var repo = new MockEdmsRepository();
        return new ValidationEngine(ArtifactRetrieval(repo), new MockDocumentVerificationService(new MockExtractionDataset()));
    }

    /// <summary>
    /// Builds a fully wired <see cref="WorkflowService"/> over the mock stack
    /// (mock AI platform, tenant-aware retrieval, in-memory persistence) for
    /// host-free controller tests.
    /// </summary>
    public static WorkflowService WorkflowService()
    {
        var mapper = new AIPayloadMapper();
        var docsRepo = new MockEdmsRepository();
        var artifactRetrieval = ArtifactRetrieval(docsRepo);
        var engine = new ValidationEngine(artifactRetrieval, new MockDocumentVerificationService(new MockExtractionDataset()));
        var aiPlatform = new LocalAiPlatformClient(mapper, Router(docsRepo), engine);
        var resultsRepo = new InMemoryVerificationResultRepository();
        return new WorkflowService(aiPlatform, mapper, artifactRetrieval, resultsRepo);
    }
}
