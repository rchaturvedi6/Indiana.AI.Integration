using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Indiana.AI.Api.Controllers;

/// <summary>
/// Stand-in File Management Server (FMS / EDMS) endpoint exposed at the real
/// contract path so the AI Verification Service can download an uploaded file
/// during validation (<c>GetEDMSFileQuery</c>, API 2). Returns seeded mock data;
/// no real Oracle / EDMS is required.
/// </summary>
[ApiController]
[Produces("application/json")]
public class FileManagementController : ControllerBase
{
    private readonly IFileManagementService _fms;

    /// <summary>Creates a new <see cref="FileManagementController"/>.</summary>
    public FileManagementController(IFileManagementService fms) => _fms = fms;

    /// <summary>
    /// File Management server request to download an uploaded file (API 2).
    /// Accepts the <c>where { edmsFileId, tenantId }</c> envelope and returns the
    /// file metadata plus Base64 content.
    /// </summary>
    /// <param name="request">The FMS query request (schema GetEdmsFileRequest).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">File found and returned (Base64 + metadata).</response>
    /// <response code="404">File not found.</response>
    [HttpPost("/R6.0/FileMgmtQuery/GetEDMSFileQuery")]
    [ProducesResponseType(typeof(GetEdmsFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetEdmsFileResponse>> GetEdmsFileAsync(
        [FromBody] GetEdmsFileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _fms.GetEdmsFileAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { code = "4004", message = ex.Message });
        }
    }
}
