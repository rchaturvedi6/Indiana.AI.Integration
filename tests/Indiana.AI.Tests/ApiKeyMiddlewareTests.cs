using Indiana.AI.Api.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Indiana.AI.Tests;

public class ApiKeyMiddlewareTests
{
    private static ApiKeyMiddleware Build(SecurityOptions options) =>
        new(_ => Task.CompletedTask, Options.Create(options));

    [Fact]
    public void Validate_passes_when_key_and_source_present()
    {
        var mw = Build(new SecurityOptions { RequireApiKey = true });
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-API-Key"] = "abc123";
        ctx.Request.Headers["X-Source"] = "provider_portal";

        Assert.Null(mw.Validate(ctx.Request));
    }

    [Fact]
    public void Validate_fails_when_api_key_missing()
    {
        var mw = Build(new SecurityOptions { RequireApiKey = true });
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Source"] = "provider_portal";

        Assert.Contains("X-API-Key", mw.Validate(ctx.Request));
    }

    [Fact]
    public void Validate_fails_when_source_missing()
    {
        var mw = Build(new SecurityOptions { RequireApiKey = true, RequireSource = true });
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-API-Key"] = "abc123";

        Assert.Contains("X-Source", mw.Validate(ctx.Request));
    }

    [Fact]
    public void Validate_rejects_key_not_in_allow_list()
    {
        var mw = Build(new SecurityOptions
        {
            RequireApiKey = true,
            RequireSource = false,
            AllowedApiKeys = { "valid-key" }
        });
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-API-Key"] = "wrong-key";

        Assert.Contains("Invalid", mw.Validate(ctx.Request));
    }

    [Fact]
    public async Task Invoke_passes_through_when_enforcement_disabled()
    {
        var called = false;
        var mw = new ApiKeyMiddleware(_ => { called = true; return Task.CompletedTask; },
            Options.Create(new SecurityOptions { RequireApiKey = false }));
        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/api/document-upload";

        await mw.InvokeAsync(ctx);

        Assert.True(called);
        Assert.Equal(200, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_returns_401_on_protected_path_without_key()
    {
        var called = false;
        var mw = new ApiKeyMiddleware(_ => { called = true; return Task.CompletedTask; },
            Options.Create(new SecurityOptions { RequireApiKey = true }));
        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/api/document-upload";

        await mw.InvokeAsync(ctx);

        Assert.False(called);
        Assert.Equal(401, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_ignores_unprotected_path()
    {
        var called = false;
        var mw = new ApiKeyMiddleware(_ => { called = true; return Task.CompletedTask; },
            Options.Create(new SecurityOptions { RequireApiKey = true }));
        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/health";

        await mw.InvokeAsync(ctx);

        Assert.True(called);
    }
}
