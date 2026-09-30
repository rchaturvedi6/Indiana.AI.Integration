using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Indiana.AI.Api.Controllers;

/// <summary>
/// AI Integration Gateway inbound API (InterChange / Provider Portal facing).
/// InterChange calls these <c>/api/*</c> routes; the gateway then maps, routes
/// and forwards to the AI Verification Service (<c>/pe/document_upload</c>,
/// <c>/pe/document_validate</c>) and the File Management Server
/// (<c>GetEDMSFileQuery</c>). Covers document upload (API 1), artifact retrieval
/// (API 2), document validation (API 3) and verification-result lookup.
/// </summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public class DocumentsController : ControllerBase
{
    private readonly IWorkflowService _workflow;

    /// <summary>Creates a new <see cref="DocumentsController"/>.</summary>
    public DocumentsController(IWorkflowService workflow) => _workflow = workflow;

    /// <summary>
    /// Document upload request (API 1). Provider Portal → AI Verification
    /// Service. Accepts the enrollment payload (route + enrollment with provider
    /// information and attachments) and returns the acknowledgement.
    /// </summary>
    /// <param name="request">The upload payload (schema DocUploadRequest).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Upload accepted (ack code 1002).</response>
    [HttpPost("document-upload")]
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UploadResponse>> UploadAsync(
        [FromBody] UploadRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _workflow.ProcessUploadAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Document validate request (API 3). Runs AI validation of the uploaded
    /// documents against the entered enrollment data, persists the per-document
    /// verification results and returns the mismatch details.
    /// </summary>
    /// <param name="request">The validate payload (schema DocValidateRequest).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Validation completed (PASS or FAIL).</response>
    [HttpPost("document-validate")]
    [ProducesResponseType(typeof(DocumentValidationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<DocumentValidationResponse>> ValidateAsync(
        [FromBody] UploadRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _workflow.ValidateAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// File Management Server file retrieval (API 2). Retrieves the binary
    /// content of an uploaded EDMS file (Base64) via the GetEDMSFileQuery
    /// envelope.
    /// </summary>
    /// <param name="request">The FMS query request (schema GetEdmsFileRequest).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">File found and returned.</response>
    /// <response code="404">File not found.</response>
    [HttpPost("artifact-retrieval")]
    [ProducesResponseType(typeof(GetEdmsFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetEdmsFileResponse>> RetrieveArtifactAsync(
        [FromBody] GetEdmsFileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _workflow.RetrieveArtifactAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            return NotFound(new { code = "4004", message = ex.Message });
        }
    }

    /// <summary>Returns the stored verification result for a file.</summary>
    /// <param name="fileId">The file identifier (GUID), for example the attachment fileId.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">The stored verification result.</response>
    /// <response code="404">No verification result stored for the file.</response>
    [HttpGet("verification-results/{fileId}")]
    [ProducesResponseType(typeof(VerificationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VerificationResult>> GetVerificationResultAsync(
        string fileId,
        CancellationToken cancellationToken)
    {
        var result = await _workflow.GetVerificationResultAsync(fileId, cancellationToken);
        return result is null
            ? NotFound(new { code = "4004", message = $"No verification result found for '{fileId}'." })
            : Ok(result);
    }
}
