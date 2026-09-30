namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Tenant-aware router for the AI Integration Gateway. Resolves which document
/// adapter services a given tenant, using the configuration-driven
/// <see cref="ITenantConfigurationProvider"/> and the adapter framework. No
/// source resolution is hardcoded.
/// </summary>
public interface IRouter
{
    /// <summary>Resolves the adapter name configured for the supplied tenant.</summary>
    string ResolveAdapterName(string? tenantId);

    /// <summary>Resolves the concrete adapter configured for the supplied tenant.</summary>
    IDocumentAdapter ResolveAdapter(string? tenantId);
}
