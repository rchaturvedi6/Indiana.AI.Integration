using Indiana.AI.Application.Abstractions;
using Indiana.AI.Application.Mapping;
using Indiana.AI.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Indiana.AI.Application;

/// <summary>
/// Dependency injection registration for the Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the Application layer services (mapper, router, validation
    /// engine and workflow orchestrator).
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAIPayloadMapper, AIPayloadMapper>();
        services.AddScoped<IRouter, Router>();
        services.AddScoped<IValidationEngine, ValidationEngine>();
        services.AddScoped<IWorkflowService, WorkflowService>();

        // Default (mock) AI platform client. AddInfrastructure replaces this
        // with the live Enso client when Integration:UseMocks is false.
        services.AddScoped<IAiPlatformClient, LocalAiPlatformClient>();
        return services;
    }
}
