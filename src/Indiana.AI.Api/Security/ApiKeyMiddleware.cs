using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Indiana.AI.Api.Security;

/// <summary>
/// Middleware that enforces the inbound AI Verification Service security
/// contract (<c>X-API-Key</c> / <c>X-Source</c>) on protected routes when
/// enforcement is enabled via <see cref="SecurityOptions.RequireApiKey"/>.
/// Disabled by default so the POC/demo runs without credentials.
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityOptions _options;

    /// <summary>Creates a new <see cref="ApiKeyMiddleware"/>.</summary>
    public ApiKeyMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
    {
        _next = next;
        _options = options.Value ?? new SecurityOptions();
    }

    /// <summary>Processes the request, short-circuiting with 401 on a contract violation.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldValidate(context))
        {
            var failure = Validate(context.Request);
            if (failure is not null)
            {
                await WriteUnauthorizedAsync(context, failure).ConfigureAwait(false);
                return;
            }
        }

        await _next(context).ConfigureAwait(false);
    }

    private bool ShouldValidate(HttpContext context)
    {
        if (!_options.RequireApiKey)
        {
            return false;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        return _options.ProtectedPathPrefixes.Any(prefix =>
            path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns a failure message when the request violates the security contract,
    /// or <c>null</c> when the request is authorized. Exposed for unit testing.
    /// </summary>
    public string? Validate(HttpRequest request)
    {
        var apiKey = request.Headers[_options.ApiKeyHeaderName].ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return $"Missing required '{_options.ApiKeyHeaderName}' header.";
        }

        if (_options.AllowedApiKeys.Count > 0 &&
            !_options.AllowedApiKeys.Contains(apiKey, StringComparer.Ordinal))
        {
            return $"Invalid '{_options.ApiKeyHeaderName}'.";
        }

        if (_options.RequireSource &&
            string.IsNullOrWhiteSpace(request.Headers[_options.SourceHeaderName].ToString()))
        {
            return $"Missing required '{_options.SourceHeaderName}' header.";
        }

        return null;
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { code = "4010", status = "FAIL", message });
        await context.Response.WriteAsync(body).ConfigureAwait(false);
    }
}
