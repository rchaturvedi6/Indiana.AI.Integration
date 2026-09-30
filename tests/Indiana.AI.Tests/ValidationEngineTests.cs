using System.Linq;
using System.Threading.Tasks;
using Indiana.AI.Application.Mapping;
using Indiana.AI.Application.Services;
using Indiana.AI.Domain.Enums;

namespace Indiana.AI.Tests;

public class ValidationEngineTests
{
    private static ValidationEngine CreateEngine() => TestFactory.ValidationEngine();

    [Fact]
    public async Task ValidateAsync_passes_for_clean_document()
    {
        var engine = CreateEngine();
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(TestData.Attachment(TestData.CleanFileId, "Federal", "Federal W-9 Form"));

        var response = await engine.ValidateAsync(request, mapper.CreateTrackingContext(request));

        Assert.Equal("2000", response.ResultsSummary.Code);
        Assert.Equal("PASS", response.ResultsSummary.Status);
        Assert.Empty(response.FailedDocs);
        Assert.Equal(1, response.ResultsSummary.TotalDocs);
        Assert.Equal(1, response.ResultsSummary.PassedDocs);
    }

    [Fact]
    public async Task ValidateAsync_reports_attachment_type_mismatch_3001()
    {
        var engine = CreateEngine();
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(TestData.Attachment(TestData.HospLicFileId, "HospLic", "Hospital License"));

        var response = await engine.ValidateAsync(request, mapper.CreateTrackingContext(request));

        Assert.Equal("2001", response.ResultsSummary.Code);
        Assert.Equal("FAIL", response.ResultsSummary.Status);
        var failed = Assert.Single(response.FailedDocs);
        var error = Assert.Single(failed.ValidationErrors);
        Assert.Equal(ValidationErrorCode.AttachmentTypeMismatch, error.Code);
        Assert.Equal("HospLic", error.Entered);
        Assert.Equal("DEA Certificate", error.Extracted);
    }

    [Fact]
    public async Task ValidateAsync_reports_field_mismatches_for_clia_document()
    {
        var engine = CreateEngine();
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(TestData.Attachment(TestData.CliaFileId, "CLIA", "CLIA certificate required"));

        var response = await engine.ValidateAsync(request, mapper.CreateTrackingContext(request));

        var failed = Assert.Single(response.FailedDocs);
        var codes = failed.ValidationErrors.Select(e => e.Code).ToList();
        Assert.Contains(ValidationErrorCode.CliaNumberMismatch, codes);   // 3116
        Assert.Contains(ValidationErrorCode.AddressMismatch, codes);      // 3109
        Assert.Contains(ValidationErrorCode.EffectiveDateMismatch, codes);// 3127
        Assert.Contains(ValidationErrorCode.ExpirationDateMismatch, codes);// 3129

        var clia = failed.ValidationErrors.First(e => e.Code == ValidationErrorCode.CliaNumberMismatch);
        Assert.Equal("15B3453646", clia.Entered);
        Assert.Equal("19D1234567", clia.Extracted);
    }

    [Fact]
    public async Task ValidateAsync_counts_multiple_documents()
    {
        var engine = CreateEngine();
        var mapper = new AIPayloadMapper();
        var request = TestData.Upload(
            TestData.Attachment(TestData.CleanFileId, "Federal", "Federal W-9 Form"),
            TestData.Attachment(TestData.HospLicFileId, "HospLic", "Hospital License"));

        var response = await engine.ValidateAsync(request, mapper.CreateTrackingContext(request));

        Assert.Equal(2, response.ResultsSummary.TotalDocs);
        Assert.Equal(1, response.ResultsSummary.FailedDocs);
        Assert.Equal(1, response.ResultsSummary.PassedDocs);
        Assert.Equal(0, response.ResultsSummary.ErrorDocs);
    }
}
