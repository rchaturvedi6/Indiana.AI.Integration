using System.Threading.Tasks;
using Indiana.AI.Application.Mapping;
using Indiana.AI.Application.Services;
using Indiana.AI.Infrastructure.Adapters;

namespace Indiana.AI.Tests;

public class LocalAiPlatformClientTests
{
    private static LocalAiPlatformClient CreateClient()
    {
        var repo = new MockEdmsRepository();
        return new LocalAiPlatformClient(new AIPayloadMapper(), TestFactory.Router(repo), TestFactory.ValidationEngine());
    }

    [Fact]
    public async Task UploadAsync_returns_ack_1002_and_echoes_route()
    {
        var client = CreateClient();
        var request = TestData.Upload(TestData.Attachment(TestData.CleanFileId, "Federal", "Federal W-9 Form"));

        var response = await client.UploadAsync(request);

        Assert.Equal("1002", response.AckStatus.Code);
        Assert.Equal("PASS", response.AckStatus.Status);
        Assert.Equal("2965796994", response.Route.RequestGUID);
    }

    [Fact]
    public async Task ValidateAsync_delegates_to_validation_engine()
    {
        var client = CreateClient();
        var request = TestData.Upload(TestData.Attachment(TestData.HospLicFileId, "HospLic", "Hospital License"));

        var response = await client.ValidateAsync(request);

        Assert.Equal("2001", response.ResultsSummary.Code);
        Assert.Single(response.FailedDocs);
    }
}
