using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Api.Controllers;
using Indiana.AI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Indiana.AI.Tests;

/// <summary>
/// Host-free tests of <see cref="DocumentsController"/> exercising APIs 1/2/3
/// through the real workflow stack (controller → workflow → validation engine →
/// tenant-aware retrieval → mock adapter → in-memory persistence).
/// </summary>
public class DocumentsControllerTests
{
    private static DocumentsController Controller() => new(TestFactory.WorkflowService());

    private static UploadRequest Upload() =>
        TestData.Upload(TestData.Attachment(TestData.HospLicFileId, "HospLic", "Hospital License"));

    [Fact]
    public async Task Upload_returns_ack_1002_and_echoes_route()
    {
        var controller = Controller();

        var action = await controller.UploadAsync(Upload(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var body = Assert.IsType<UploadResponse>(ok.Value);
        Assert.Equal("1002", body.AckStatus.Code);
        Assert.Equal("PASS", body.AckStatus.Status);
        Assert.Equal("2965796994", body.Route.RequestGUID); // echoed
    }

    [Fact]
    public async Task Validate_returns_failed_docs_and_persists_result()
    {
        var controller = Controller();
        var request = Upload();

        var action = await controller.ValidateAsync(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var body = Assert.IsType<DocumentValidationResponse>(ok.Value);
        Assert.Equal("2001", body.ResultsSummary.Code);
        var failed = Assert.Single(body.FailedDocs);
        var error = Assert.Single(failed.ValidationErrors);
        Assert.Equal("3001", error.Code);
        Assert.Equal("HospLic", error.Entered);
        Assert.Equal("DEA Certificate", error.Extracted);

        // Persistence: the stored result is retrievable via the same controller.
        var lookup = await controller.GetVerificationResultAsync(TestData.HospLicFileId, CancellationToken.None);
        var lookupOk = Assert.IsType<OkObjectResult>(lookup.Result);
        var stored = Assert.IsType<VerificationResult>(lookupOk.Value);
        Assert.Equal("7807176902", stored.TrackingId);
    }

    [Fact]
    public async Task ArtifactRetrieval_returns_base64_and_metadata_for_tenant()
    {
        var controller = Controller();
        var request = new GetEdmsFileRequest
        {
            Where = new EdmsWhere { EdmsFileId = "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", TenantId = "Indiana" }
        };

        var action = await controller.RetrieveArtifactAsync(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var body = Assert.IsType<GetEdmsFileResponse>(ok.Value);
        Assert.Equal("2.Federal_W9_Tax_Form.pdf", body.QueryResult.FileName);
        Assert.False(string.IsNullOrEmpty(body.QueryResult.FileData));
        Assert.NotEmpty(body.QueryResult.KeyIdentifiers);
    }

    [Fact]
    public async Task ArtifactRetrieval_returns_404_for_unknown_file()
    {
        var controller = Controller();
        var request = new GetEdmsFileRequest
        {
            Where = new EdmsWhere { EdmsFileId = "00000000-0000-0000-0000-000000000000", TenantId = "Indiana" }
        };

        var action = await controller.RetrieveArtifactAsync(request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(action.Result);
    }

    [Fact]
    public async Task GetVerificationResult_returns_404_when_absent()
    {
        var controller = Controller();

        var action = await controller.GetVerificationResultAsync("missing", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(action.Result);
    }
}
