using System.Collections.Generic;

namespace Indiana.AI.Infrastructure.Configuration;

/// <summary>
/// Binds the <c>Routing</c> configuration section. Drives tenant-aware,
/// configuration-driven adapter resolution (for example
/// <c>Indiana → IndianaOracleAdapter</c>).
/// </summary>
public class RoutingOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Routing";

    /// <summary>
    /// Tenant used when an inbound tenant id is not found in <see cref="Tenants"/>
    /// (for example a tenant GUID that maps to the primary state). Optional.
    /// </summary>
    public string? DefaultTenant { get; set; }

    /// <summary>
    /// Tenant → adapter-name map, for example
    /// <c>{ "Indiana": "IndianaOracleAdapter", "Nevada": "SqlAdapter" }</c>.
    /// </summary>
    public Dictionary<string, string> Tenants { get; set; } = new();
}
