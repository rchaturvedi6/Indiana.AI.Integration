using System;
using System.Collections.Generic;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Indiana.AI.Infrastructure.Routing;

/// <summary>
/// Reads the tenant→adapter routing map from application settings (the
/// <c>Routing</c> section) and resolves the adapter name for a tenant. This is
/// the single source of tenant resolution — no mapping is hardcoded in code.
/// </summary>
public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private readonly Dictionary<string, string> _tenants;
    private readonly string? _defaultTenant;

    /// <summary>Creates a new <see cref="TenantConfigurationProvider"/>.</summary>
    public TenantConfigurationProvider(IOptions<RoutingOptions> options)
    {
        var value = options.Value ?? new RoutingOptions();
        _tenants = new Dictionary<string, string>(value.Tenants, StringComparer.OrdinalIgnoreCase);
        _defaultTenant = value.DefaultTenant;
    }

    /// <inheritdoc />
    public string ResolveAdapterName(string? tenantId)
    {
        if (!string.IsNullOrWhiteSpace(tenantId) && _tenants.TryGetValue(tenantId.Trim(), out var adapter))
        {
            return adapter;
        }

        if (!string.IsNullOrWhiteSpace(_defaultTenant) && _tenants.TryGetValue(_defaultTenant, out var fallback))
        {
            return fallback;
        }

        throw new InvalidOperationException(
            $"No adapter is configured for tenant '{tenantId}', and no usable default tenant is set. " +
            $"Configured tenants: {string.Join(", ", _tenants.Keys)}.");
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> GetMappings() => _tenants;
}
