using Indiana.AI.Application.Abstractions;

namespace Indiana.AI.Application.Services;

/// <summary>
/// Tenant-aware <see cref="IRouter"/> implementation. Resolves the destination
/// adapter for a tenant using the configuration-driven
/// <see cref="ITenantConfigurationProvider"/> and the adapter framework
/// (<see cref="IAdapterResolver"/>). Contains no hardcoded source resolution.
/// </summary>
public class Router : IRouter
{
    private readonly ITenantConfigurationProvider _tenants;
    private readonly IAdapterResolver _adapterResolver;

    /// <summary>Creates a new <see cref="Router"/>.</summary>
    public Router(ITenantConfigurationProvider tenants, IAdapterResolver adapterResolver)
    {
        _tenants = tenants;
        _adapterResolver = adapterResolver;
    }

    /// <inheritdoc />
    public string ResolveAdapterName(string? tenantId) => _tenants.ResolveAdapterName(tenantId);

    /// <inheritdoc />
    public IDocumentAdapter ResolveAdapter(string? tenantId) =>
        _adapterResolver.Resolve(ResolveAdapterName(tenantId));
}
