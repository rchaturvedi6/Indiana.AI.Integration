using System;
using Indiana.AI.Infrastructure.Adapters;

namespace Indiana.AI.Tests;

public class RouterTests
{
    [Theory]
    [InlineData("Indiana", "IndianaOracleAdapter")]
    [InlineData("Nevada", "SqlAdapter")]
    [InlineData("FutureState", "MockFileAdapter")]
    public void ResolveAdapterName_maps_tenant_to_configured_adapter(string tenant, string expected)
    {
        var provider = TestFactory.TenantProvider();

        Assert.Equal(expected, provider.ResolveAdapterName(tenant));
    }

    [Fact]
    public void ResolveAdapterName_falls_back_to_default_tenant_for_unknown_tenant()
    {
        var provider = TestFactory.TenantProvider(defaultTenant: "Indiana");

        // A tenant GUID that isn't in the map resolves to the default tenant's adapter.
        Assert.Equal("IndianaOracleAdapter", provider.ResolveAdapterName("b37b1cec-6603-2d4f-ac37-74e0724774a1"));
    }

    [Fact]
    public void ResolveAdapterName_throws_when_unknown_and_no_default()
    {
        var provider = TestFactory.TenantProvider(defaultTenant: null);

        Assert.Throws<InvalidOperationException>(() => provider.ResolveAdapterName("Ohio"));
    }

    [Fact]
    public void Router_resolves_correct_adapter_instance_per_tenant()
    {
        var repo = new MockEdmsRepository();
        var router = TestFactory.Router(repo);

        Assert.IsType<IndianaOracleAdapter>(router.ResolveAdapter("Indiana"));
        Assert.IsType<SqlAdapter>(router.ResolveAdapter("Nevada"));
        Assert.IsType<MockFileAdapter>(router.ResolveAdapter("FutureState"));
    }
}
