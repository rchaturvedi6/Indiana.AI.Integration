using Indiana.AI.Application.Mapping;

namespace Indiana.AI.Tests;

public class AIPayloadMapperTests
{
    [Fact]
    public void CreateTrackingContext_derives_ids_from_route_and_enrollment()
    {
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(TestData.Attachment(TestData.CleanFileId, "Federal", "Federal W-9 Form"));

        var tracking = mapper.CreateTrackingContext(request);

        Assert.Equal("7807176902", tracking.TrackingId);   // enrollment.trackingID
        Assert.Equal("2965796994", tracking.CorrelationId); // route.requestGUID
        Assert.Equal("b37b1cec-6603-2d4f-ac37-74e0724774a1", tracking.TenantId);
        Assert.Equal("prov_portal_ftest", tracking.AppClient);
    }

    [Fact]
    public void CreateTrackingContext_generates_ids_when_missing()
    {
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload();
        request.Enrollment.TrackingID = "";
        request.Route.RequestGUID = "";

        var tracking = mapper.CreateTrackingContext(request);

        Assert.StartsWith("TRK-", tracking.TrackingId);
        Assert.StartsWith("COR-", tracking.CorrelationId);
    }

    [Fact]
    public void Map_projects_enrollment_onto_ai_payload()
    {
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(TestData.Attachment(TestData.CleanFileId, "Federal", "Federal W-9 Form"));
        var tracking = mapper.CreateTrackingContext(request);

        var payload = mapper.Map(request, tracking);

        Assert.Equal("1269201798", payload.ProviderNpi);
        Assert.Equal("Inuvik Regional Hospital Research", payload.ProviderLegalName);
        var doc = Assert.Single(payload.Documents);
        Assert.Equal(TestData.CleanFileId, doc.FileId);
        Assert.Equal("Federal", doc.AttachmentTypeCode);
    }
}
