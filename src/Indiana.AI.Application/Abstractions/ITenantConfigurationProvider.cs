using System.Collections.Generic;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Supplies the tenant→adapter routing configuration read from application
/// settings. Enables tenant-aware, configuration-driven adapter resolution with
/// no hardcoded source mapping.
/// </summary>
public interface ITenantConfigurationProvider
{
    /// <summary>
    /// Resolves the configured adapter name for a tenant (for example
    /// "Indiana" → "IndianaOracleAdapter"). Falls back to the configured default
    /// tenant when the supplied tenant is unknown; throws when neither resolves.
    /// </summary>
    string ResolveAdapterName(string? tenantId);

    /// <summary>Returns the full tenant→adapter map.</summary>
    IReadOnlyDictionary<string, string> GetMappings();
}
