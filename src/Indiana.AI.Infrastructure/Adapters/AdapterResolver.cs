using System;
using System.Collections.Generic;
using System.Linq;
using Indiana.AI.Application.Abstractions;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// The adapter framework entry point. Resolves a registered
/// <see cref="IDocumentAdapter"/> by its <see cref="IDocumentAdapter.Name"/>.
/// </summary>
public class AdapterResolver : IAdapterResolver
{
    private readonly Dictionary<string, IDocumentAdapter> _adapters;

    /// <summary>
    /// Creates a resolver over the registered adapters, keyed by name
    /// (case-insensitive).
    /// </summary>
    public AdapterResolver(IEnumerable<IDocumentAdapter> adapters)
    {
        _adapters = adapters.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public IDocumentAdapter Resolve(string adapterName)
    {
        if (string.IsNullOrWhiteSpace(adapterName) || !_adapters.TryGetValue(adapterName, out var adapter))
        {
            throw new InvalidOperationException(
                $"No document adapter is registered with name '{adapterName}'. " +
                $"Registered adapters: {string.Join(", ", _adapters.Keys)}.");
        }

        return adapter;
    }
}
